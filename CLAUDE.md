# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 다음 작업 (2026-07-26 갱신)

### 완료 (이번 세션)
- **HpUI 배치** (`characterId` 0/1) — 데미지가 실제 HP 에 반영되는 것 눈으로 확인.
- **주사위 적재 배선** — `BoutStartEvent.Apply` 가 `ResolveCombat` 앞에서 `UseAction(A)` / `UseAction(B)` 호출.
  이 단계가 통째로 빠져 있어(호출자 0건) 주사위가 안 실리고 데미지가 0 이었음.
- **이벤트 실행 순서 교정** — `DamageEvent` / `StatusDamageEvent` / `StaggerEvent` 를 "상태 먼저 → `AddLog`" 로.
  로그가 먼저 나가면 그걸 받아 모델을 읽는 UI 가 변경 전 값을 그려 한 박자 밀렸음. (DEVLOG 2026-07-22, 핵심 규칙 갱신)
- **에너지(빛) 회복 — 구 1번 (게임플레이 블로커) 해결** —
  `CharacterRuntime.RecoverEnergy(amount)` 신설(최대치 클램프), `EnergyRecoverEvent`/`EnergyRecoverLog` 추가,
  `TurnStartEvent.Apply` 가 `TriggerTurnStart` **앞에서** +1 회복(턴 시작 훅이 회복 전 값을 보면 안 되므로).
  회복량은 일단 `1` 하드코딩 — "다음 작업 5"(드로우 장수) 때 같은 패턴으로 데이터화.
  **주의: LOR 기본은 "매 턴 최대 복구"가 아니라 매 씬 +1.** 이전 판에 "매 턴 최대로 복구(LOR 기본)" 이라고
  적혀 있던 것은 오류였음(사용자가 교정).
- **감정 레벨업 시 빛 전액 회복** — `EmotionLevelUpEvent` 가 부족분(`MaxEnergy - CurrentEnergy`)을 계산해
  `EnergyRecoverEvent` 를 enqueue. 로그의 `Amount` 가 요청량이 아니라 **실제 회복량**을 기록하는 방식(B안).
  0 이면 enqueue 자체를 생략해 로그 노이즈 차단.
- **일방 공격 수정 — 구 2번 (버그) 해결, 플레이 검증 완료** — `CombatExecutor.RunQueue` 가
  `bool hasEdge` 로 edge 조회 결과를 받고, 없으면 `action.TargetSlot` 폴백. 생존 검사는 공통 통과,
  클래시 조건에 `hasEdge` 추가(edge 없이 타겟이 제3자를 노리면 합이 아니라 일방으로 가야 하므로 필수).
  이제 루프의 모든 탈출 경로가 `visited.Add(slot)` 을 찍는다.
  함정이었던 것: struct 인 `SpeedSlot` 은 `TryGetValue` 실패 시 null 이 아니라 `default`(0번 캐릭터 슬롯)가
  나와서, 폴백 없이 흘리면 무조건 0번을 때린다.
- **클래시 데미지를 원작 규칙으로 교정** — `DiceRuleTable` 의 승자 수치를 "차이" → **굴림값 전체**로.
  단 매치업별 예외 유지: Attack 이 Block 을 이기면 `공격값 - 수비값`(방어 경감, 원작 규칙),
  Counter vs Block 도 동일 대칭. 막기 승리 스태거 피해와 회피 승리 회복은 전부 굴림값 전체.
  이제 합/일방(`ResolveUnopposedDice` 는 원래 전체) 데미지 스케일이 일치한다.
- **`BoutGraph.Clear()` 가 `actionBySlot` 도 비우도록 수정 — 구 3번 해결** — 슬롯 표시 잔류와
  다음 턴 스테일 액션 여지 제거. 이 사전은 `BattleRuntime` 이 즉석 생성해 넘긴 것이라 외부 소유자 없음(지워도 안전).
- **HpUI 의 진단용 `Debug.Log` 제거** — `characterId` 필터 앞에 있어 HpUI 인스턴스 수만큼(2번씩) 찍히던
  "데미지 로그 중복"의 원인. 엔진 이중 데미지가 아니었음. 콘솔 로그가 필요하면 별도 로그 콘솔 컴포넌트가 맞는 자리.
- **[4-1단계] 카드 파이프라인 `CardData` → `CardModel` 치환 완료 (2026-07-23, 플레이 검증)** —
  순수 클래스 `CardModel`(`CardName`/`Cost`/`Dices`, 전부 get 전용) 신설, `CardData.ToModel()` 이 변환 담당.
  `CardZone`/`CardManager`/`CardResolver`/`ActionInstance`/카드 Event·Log 전부 교체.
  **Engine 폴더의 `CardData` 타입 참조 0.** 변환 관문은 `CharacterState` 생성자(SO 덱 → 모델 덱).
  - artwork(Sprite)는 UI 전용으로 분리: `BattleManager` 가 `CreateBattle` 때 이름→Sprite 레지스트리를 만들고
    `GetCardArtwork(name)` 노출, `CardHandUI` 가 조회해 `CardUI.Setup` 3번째 인자로 주입. `CardUI` 는 레지스트리를 모름.
  - `CardData` 는 `[SerializeField] private` + 프로퍼티로 정리(스타일 위반 해소). 필드 리네임에 `FormerlySerializedAs`
    필수였음 — **어트리뷰트에 넣는 이름은 에셋 파일(디스크)에 실제 적힌 키** (Strike.asset 리셋 직전에 잡음).
- **상태이상 만료 처리 수정 + duration 배선 (2026-07-25~26, 플레이 검증 완료)** —
  DEVLOG `2026-07-25 ~ 07-26` 참고. 요약:
  - `FlushExpired` 가 `_statusEffects` 에서도 제거하도록 수정. 맵 제거는 `ReferenceEquals` 로 동일 인스턴스 확인 후에만.
  - 8개 `Trigger*` 를 `ToArray()` 스냅샷 + `if (effect.IsExpired) continue` 로. `FlushExpired` 위치를
    "상태이상 루프 직후 → 패시브 루프" 로 통일, `TriggerTurnEnd` 에 빠져 있던 `EnsureSorted()` 추가.
  - `AddStatus` 에 `!effect.IsExpired` 가드 — 만료 예정 좀비에 스택이 쌓여 증발하던 경로 차단.
  - `duration` 배선: 생성자 인자 + `public const int Permanent = -1`. **`TickTurnEnd`(비-virtual 래퍼)**가
    Duration 감소를 독점하고 `OnTurnEnd` 는 `protected` 로 내려 우회를 컴파일 에러로 만듦.
    값은 Bleed/Burn = `Permanent`(스택 소모형), Strength/Paralysis = `1`(턴 카운트형).
  - `Refresh()` + `readonly _baseDuration`. **단 현재 규칙에선 영구 no-op** — duration 이 1이나 Permanent 뿐이라
    값이 안 바뀐다. 원작에 "N턴 지속" 버프가 없으므로 앞으로도 안 바뀔 가능성이 높다(데드 코드 정리 후보).
  - `StatusAddEvent` / `StatusAddLog` 신설. `AddStatus` 가 **결과 스택**을 반환해 로그에 실음(EnergyRecover B안과 동일).
- **3-1 검증 완료 (2026-07-26)** — DEVLOG `2026-07-26` 참고. `BattleManager` 의 임시 `DebugAdd*` 3개로
  진입점을 뚫어 6개 항목 전부 통과: 출혈 5→2→1(재진입 경로에서 증발 없음) / 힘 굴림 +3 /
  duration 1 만료 / 힘 2회 스택 6 / 마비 -2 / priority 힘→마비 순서(= `EnsureSorted` 도 같이 검증됨).
  힘·마비는 관측 창구가 없어 `StatusEffects.cs:39` 브레이크포인트 + Locals 로 확인.
  **`DebugAdd*` 는 3-2(Delay) 검증에 또 쓰므로 아직 남겨둠.** 그때 삭제.
- **잔재 정리 일부 완료 (2026-07-26)** — `BattleRuntime.Start(BattleInput)` + `_input` 제거,
  `CharacterState.Source` 제거, 그 결과 참조 0이 된 `BattleInput.cs` / `BattleResult.cs` 삭제.
- **3-1.5 출혈 발동 조건 교정 완료 (2026-07-26, 플레이 검증)** — "합마다"에서 **"주사위를 굴릴 때마다"**로.
  `ResolveUnopposedDice` 의 `Roll` 직후에 `attacker.TriggerDiceRoll()` 한 줄 + `OnDiceClash`/`TriggerDiceClash`
  → `OnDiceRoll`/`TriggerDiceRoll` 개명 5곳. 범위는 주사위 4종 전부라 타입 검사 없음.
  대조군(고치기 전 일방에서 안 터짐) → 수정 후 터짐 → 합 5→2→1 회귀 없음 순으로 확인.
  - **방어·회피 주사위 발동은 아직 미검증** — 카드가 `Strike`(Attack 1개)뿐이라 만들 수가 없다. 카드가 늘어난 뒤로.

### 3. 상태이상 — Delay(발효 지연) (진행 중)

- **3-1.6. 상태이상으로 죽어도 그 주사위의 공격이 나간다** — `RunQueue` 의 `IsValidAction`(IsDead 검사)은
  액션을 큐에서 꺼낼 때 한 번만 돈다. 그래서 출혈로 공격자가 죽어도 그 굴림의 `DamageEvent` 는 그대로 나간다.
  **3-1.5 가 만든 버그가 아니라 클래시 경로에 원래 있던 동작**이다
  (`CombatExecutor.cs:134` 에서 터지고 137~144 에서 이벤트가 나감).
  원작 규칙 확인 필요 — 죽은 시점에 남은 주사위가 소멸하는 게 맞다면 `ResolveCombat` 의 `while` 루프에
  생존 검사를 넣는 방향.
- **3-2. Delay 필드 + `Tick*` 래퍼 7개** — "다음 턴에 힘 부여" 류. **duration 과 다른 축이다**:
  duration = 얼마나 오래, delay = 언제 시작. 발효 시점은 **카드의 성질**이라(즉시 부여 카드와 다음 턴
  부여 카드가 공존) 효과 클래스에 하드코딩하면 안 되고 `StatusAddEvent` 까지 인자로 뚫어야 한다.
  - 나머지 7개 훅도 `TickTurnEnd` 와 같은 패턴으로: 래퍼가 `IsActive` 가드 → `protected virtual On*` 위임
  - `AddStatus` 의 기존-효과 분기에서 `Delay = Min(기존, 신규)` 병합 필요 (비활성 힘에 즉시 힘이 겹칠 때)
  - 원작 경우의 수는 **이번 턴 / 다음 턴 / 영구 3가지뿐**(사용자 확인). 현재 `int` 표현이 그 상위집합이라
    재설계는 불필요. 굳이 좁히면 `enum StatusDuration` 이지만 지금 것도 틀리지 않았다.
- **3-3. 훅 순회를 스냅샷 → 재사용 없는 인덱스 루프로** — `ToArray()` 할당 제거.
  **공유 버퍼 필드 하나로 돌려쓰면 재진입 때문에 깨진다**(바깥 루프가 쓰던 배열을 안쪽이 덮어씀).
  올바른 방향은 `_triggerDepth` 카운터로 "순회 중엔 제거·정렬 금지"를 강제하는 것:
  - `FlushExpired` / `EnsureSorted` 맨 앞에 `if (_triggerDepth > 0) return;` (`EnsureSorted` 는 `_dirty` 를 내리지 않는다)
  - 호출 순서가 함정: `EnsureSorted()` → `_triggerDepth++` → `try { 인덱스 루프 } finally { _triggerDepth--; }` → `FlushExpired()`.
    `EnsureSorted` 를 `++` 뒤에 두면 최외곽도 정렬을 건너뛰고, `FlushExpired` 를 `finally` 안에 두면 영영 안 지워진다
  - `finally` 필수 — 예외로 depth 가 안 내려가면 그 캐릭터는 이후 전투 내내 정렬·만료가 멈춘다
  - 실익은 성능이 아니라 구조다(리스트가 비어 있어 지금은 할당도 거의 없음). 우선순위 낮음
- **3-4. 상태이상 UI** — `CharacterRuntime._statusEffects` 가 완전히 private 이라 UI 가 읽을 경로가 없다.
  `SpeedSlots` 처럼 `IReadOnlyList` 노출 필요. `StatusAddLog` 구독자도 아직 없음.

### 4. Engine → Data 의존 끊기 — 구 2번 (1단계 완료, 2~4단계 남음)

남은 SO 누수 (2026-07-23 전수조사 기준):
- `CharacterData` ← `CharacterState`, `CharacterStateBuilder`, `BattleSnapShot`
- `PassiveData` ← `CharacterState`, `PassiveFactory` (CLAUDE.md 구판 목록에 빠져 있던 누수)

진행 계획:
- **2단계**: `CharacterModel` 신설 + `CharacterData.ToModel()`. `CharacterState`/`Builder`/`BattleSnapShot` 이
  순수 모델을 받게. 참조 0건인 `CharacterState.Source` 프로퍼티도 이때 삭제.
  `CharacterData` 는 이미 private 필드 + 프로퍼티 구조라 직렬화 마이그레이션 불필요.
- **3단계**: `PassiveModel`(추상 + 타입별 서브클래스) 신설, `IStatModifierPassive` 구현을 모델 쪽으로 이사,
  `PassiveFactory` 가 모델을 받게. SO 는 `ToModel()` 만 갖는 껍데기로.
- **4단계**: Engine asmdef 신설 + **No Engine References** 켜서 기계 검증.

### 5. 카드 뽑기 장수를 상수 → 변수로 — 구 3번
`Engine/Events/TurnStartEvent.cs` 가 `new DrawCardEvent(CharacterId, 1)` 로 고정.
"다음 턴 카드 +n" 같은 효과를 만들 때 두 개가 필요해진다:
- 기본 장수 → `CharacterData` → `CharacterState` → `CharacterRuntime`
- 일시 보정 → `StatusEffectRuntime` 이 이미 `OnTurnStart` 훅과 만료 처리를 갖고 있으므로 그쪽이 적합

### 6. 잔재 정리 — 구 4번
- `UI/Slot/SpeedSlotUI.cs` — 참조 0건. `SlotDebugItem` 이 대체함
- `TurnUI` 의 `endTurnButton` / `turnText` 필드 — 선언만 되고 쓰이지 않음
- `BattleRuntime.HasEvents` — 참조 0건. 배수 루프를 염두에 뒀던 흔적이라 7번과 같이 판단할 것
- ~~`BattleInput.cs` / `BattleResult.cs` / `BattleRuntime.Start`~~ — 2026-07-26 삭제 완료

### 7. `Step()` 재귀 구조 (급하지 않음) — 구 5번
`BattleRuntime.EnqueueEvent` 가 enqueue 직후 `Step()` 을 호출하고, `Step()` 은 다른 곳에서
호출되지 않는다. 따라서 **큐에 원소가 2개 이상 쌓이지 않으며**, 사실상 `ev.Apply(this)` 와
등가다. 이벤트는 깊이 우선(DFS)으로 즉시 처리된다.

- 의도(파생 이벤트가 전부 처리됨)는 달성되나 메커니즘은 큐 배수가 아니라 재귀
- `HasEvents` 는 아무도 쓰지 않고 외부에서 항상 `false` — 배수 루프를 염두에 뒀던 흔적
- 부작용: 전투 종료 후 남은 `DeathEvent` 가 처리되면 `BattleEndLog` 중복 가능
- 고치면 DFS → BFS 로 실행 순서가 바뀌므로 전투 로그 비교 검증 필요

---

## 작업 방식 (최우선 — 다른 지시보다 우선함)

**Claude 는 코드를 작성하지 않는다.** 구현은 전부 사용자가 직접 한다.
사용자가 실력을 기르는 것이 이 프로젝트의 목적 중 하나이므로, 대신 짜주는 것은 그 목적을 해친다.

Claude 가 하는 일:
- 코드 읽기, 원인 분석, 진단, 영향 범위 조사
- 설계 선택지 제시와 트레이드오프 설명 (권장안 포함)
- 사용자가 작성한 코드에 대한 리뷰와 피드백
- 어디를 어떻게 고쳐야 하는지 **말로** 설명. 방향 제시용 짧은 코드 조각(시그니처, 한두 줄)은 허용
- 커밋 메시지 작성, git 작업, 문서 갱신
- 인코딩 변환 같은 기계적 일괄 작업 (배울 게 없는 순수 노동)

Claude 가 하지 않는 일:
- `Write` / `Edit` 로 `.cs` 파일 생성·수정
- "제가 구현할까요?" 제안
- 파일 통째 재작성 코드 블록 제공

사용자가 **명시적으로** 요청할 때만 예외. 애매하면 물어볼 것.

## 프로젝트 개요
Unity / C# 카드 + 주사위 전투 시스템 (LOR 스타일).
3계층: `CharacterData`(청사진) → `CharacterState`(스냅샷) → `CharacterRuntime`(가변).

**목표: Unity 에 최대한 의존하지 않는 자체 전투 엔진.**
`Engine/` 에는 `using UnityEngine` 이 한 줄도 없다. 남은 누수는 위 "다음 작업 4".

## 현재 상태 (2026-07-22)

**전투 루프가 끝에서 끝까지 동작한다.** 씬 세팅 완료, 플레이로 확인됨.

```
전투 생성 → 턴 시작(속도 굴림 + 카드 1장 뽑기)
  → 슬롯 클릭 → 카드 드래그 → 상대 슬롯에 드롭 → 합 성립(BoutGraph edge)
    → 턴 종료 버튼 → 전투 해석 → 다음 턴
```

확인된 것: 합이 잡히면 양쪽 슬롯이 서로를 가리킨다(`Bout: 1-0` / `Bout: 0-0`).
**데미지도 실제로 HP 에 반영되는 것까지 확인됨**(HpUI + 주사위 적재 배선 + 이벤트 순서 교정, 2026-07-22).
에너지는 턴 시작 +1 / 감정 레벨업 시 전액 회복으로 해결됨.
단, 클래시 데미지가 굴림값 "차이"라 작음 — "결정 필요" 참고.

## 폴더 구조

```
Assets/
├── Scripts/
│   ├── Engine/   순수 C# — Battle, Bout, Cards, Characters, Combat,
│   │             Contexts, Dice, Events, Logs, Passives, Status, Support
│   ├── Data/     ScriptableObject 정의 (CharacterData, CardData, Passives/)
│   ├── Scene/    MonoBehaviour — BattleManager, BattleStarter, Character, Player, Enemy
│   └── UI/       Card/, Slot/, Status/
├── Data/         에셋 — Cards/Strike, Characters/Ally01·Enemy01
└── Prefabs/      Card.prefab, SlotItem.prefab
```

폴더가 의존 방향을 나타낸다. `Engine` → `Data` 참조가 남아 있는 것이 "다음 작업 2".

## 구조 요약

```
Player  (씬 오브젝트. DontDestroyOnLoad 는 미구현)
  └── IReadOnlyList<Character> Roster / SelectedParty
Enemy
  └── IReadOnlyList<Character> Members
Character (MonoBehaviour, 전투 씬 한 명당 하나)
  ├── [SerializeField] CharacterData _characterData
  └── CharacterRuntime Runtime { get; private set; }
```

- `BattleStarter` → `BattleManager.CreateBattle(SelectedParty, Members)` — 두 리스트로 진영 구분
- `BattleManager` 가 `Character` → `CharacterData` 추출(즉시 평가 + null 검증) 후 엔진에 전달
- 진영은 `enum Team { Ally, Enemy }`. `CharacterState` 보유 → `CharacterRuntime` 위임
- `BattleRuntime.TryGetBattleResult(out Team? winner)` — 한쪽 전멸 시 true. 양측 전멸이면 `winner == null`
- 룰 엔진은 `DiceRuleTable`(4×4 매트릭스) + `CombatExecutor`. `IRuleSet` 계열은 폐기됨
- 클래시 결과는 `ClashContextEvent` 가 `DamageContext` / `StaggerContext` 로 분기

## 씬 세팅 참고

인스펙터 필드는 타입에 따라 넣을 것이 다르다. 계속 헷갈리는 지점:

| 필드 타입 | 넣을 것 |
|---|---|
| `Character`, `BattleManager` 등 컴포넌트 | **씬(Hierarchy) 오브젝트** |
| `CharacterData`, `CardData` | **에셋(Project 창)** |
| `GameObject cardPrefab` | **프리팹 에셋** — `Strike.asset` 은 안 들어감 |

- `CardUI.card` / `input` 은 인스펙터에서 채우지 않는다. `CardHandUI.Refresh` 가 `Setup()` 으로 주입
- 버튼 연결은 버튼 쪽 `On Click ()` 에서 한다. `TurnUI.endTurnButton` 필드는 코드에서 쓰이지 않음
- 프리팹 필수 컴포넌트: `CardUI` → `CanvasGroup`, `SlotDebugItem` → `Image`(레이캐스트용)
- `SlotDebugItem` 의 `Button` 은 `Transition = None`. 아니면 `UpdateColor` 가 칠한 합 표시를 덮어씀

## 핵심 규칙
- **상태 변경은 Event.Apply 안에서만.** 순서는 `CharacterRuntime 상태 변경 → runtime.AddLog → 후속 Event Enqueue`.
  - **상태를 먼저 바꾸고 그다음에 로그를 낸다.** `AddLog` 은 동기라 그 자리에서 UI 콜백까지 실행되는데,
    UI 는 로그를 받고 모델(`CurrentHp` 등)을 다시 읽는다. 로그를 상태 변경보다 먼저 내면 UI 가 **변경 전 값**을 읽어
    한 박자 밀린다 — `DamageEvent` 에서 실제로 겪은 버그(2026-07-22). `EnergyUse`/`ChangeMaxHp`/`Draw` 등 다수가 이미 상태-먼저.
- **CombatExecutor = 순수 계산.** 부작용 금지.
- **Data 계층은 Runtime 모르게.**
- **UI 는 LogDispatcher 구독.** `OnBattleCreated` 로 바인딩, `OnBattleEnded` 로 해제. (`SlotDebugPanel` 참고)
- **Deterministic.** RNG 는 `IRng` 통해서만, Seed 기반.
- **`IRng.Range` 는 max 포함.** Unity 의 `Random.Range(int, int)` 는 max 제외라 반대다.
  Fisher-Yates 셔플에 `Range(0, i + 1)` 을 쓰면 범위를 벗어난다 — 실제로 겪은 버그.
- **데드 코드 격리 후 삭제.** 남길 거면 `_legacy/` 폴더로.

## 네이밍 / 스타일
- 파일명 = 클래스명.
- public 필드 금지, `{ get; private set; }` 또는 `IReadOnlyList<>`.
- 메서드는 PascalCase.
- Event/Log 1:1 매칭 유지. 새 Log 추가 시 대응 Event 도 같이.
- 한글 주석 OK, 단 UTF-8. `.editorconfig` 가 `*.cs` 를 `utf-8-bom` 으로 고정한다.

## 데드 코드 판단 시 주의

**"참조 0건"만으로 판단하지 말 것. 컴파일되는지 반드시 확인.**

`Bout.cs` 를 "참조는 없지만 개념이 살아있다"는 이유로 남겼는데, 그 파일은
`ActionInstance.Speed`(존재한 적 없는 멤버)를 읽고 있어 `721b912` 부터 빌드를 깨뜨리고 있었다.
컴파일 실패 상태에서는 Unity 가 **컴포넌트 부착을 거부**하므로,
"컴포넌트가 안 붙는다"는 증상이 나오면 먼저 컴파일 에러를 의심할 것.

에러 확인 경로 (Unity 콘솔을 못 볼 때):
```
C:\Users\a0103\AppData\Local\Unity\Editor\Editor.log
```
`grep "error CS"` — 단 이 로그는 누적되므로 **삭제된 파일의 과거 에러도 섞여 있다.**
해당 파일·멤버가 현재도 존재하는지 교차 확인할 것.

## 참고 문서
- `DEVLOG.txt` — 날짜별 개발 기록. 중요한 실수는 `문제 / 해결 / 배운 점` 형식으로 남긴다.
- `DEVLOG_Summary.txt` — 1~6단계 설계 의도.
- ~~`REFACTORING_DIAGNOSIS.md`~~ — 저장소에 존재하지 않음.
