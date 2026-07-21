# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 다음 작업 (2026-07-22 기준)

### 0. HpUI 배치 — 엔진 손대기 전에 먼저
`UI/Status/HpUI.cs` 를 씬에 캐릭터당 하나씩(`characterId` 0, 1) 배치.
합이 잡히고 턴이 넘어가는 것까지는 확인했지만, **데미지가 실제로 들어가는지는 아직 아무도 못 봤다.**
엔진을 고치기 시작하면 "내가 뭘 깨뜨렸나"를 판단할 기준이 필요하므로,
정상 동작이 어떤 모습인지 먼저 봐 둘 것.

### 1. 일방 공격이 동작하지 않음 (버그)
`Engine/Combat/CombatExecutor.cs:55`

```csharp
if (!graph.edges.TryGetValue(slot, out var targetSlot)) continue;
```

타겟을 `action.TargetSlot` 이 아니라 `graph.edges` 에서 찾는다. edge 는 양쪽이 모두
액션을 등록해야 생기므로, **상대가 카드를 내지 않은 슬롯을 때리면 그냥 건너뛴다.**
LOR 규칙상 편면공격은 그대로 들어가야 하고, `ResolveUnopposedDice` 와
`BoutStartEvent(action, null, ...)` 경로는 이미 구현돼 있는데 도달하지 못하는 상태.

방향: edge 를 1순위로 보되 없으면 `action.TargetSlot` 으로 폴백.
주의: 지금 55행의 `continue` 는 `visited.Add(slot)` 도 하지 않고 넘어간다. 폴백을 넣으면
그 경로가 사라지므로 `visited` 처리를 어떻게 할지 같이 정해야 한다.

### 2. Engine → Data 의존 끊기
`Engine/` 은 `using UnityEngine` 이 없지만 `CharacterData` / `CardData`
(둘 다 ScriptableObject)를 타입으로 참조한다. `CharacterState.Source`,
`CharacterState.InitialDeck`, `ActionInstance.Card`, `CardManager`, `CardResolver` 가 해당.

방향: SO 는 "에디터에서 값을 채우는 껍데기"로 두고, 순수 클래스로 변환해 엔진에 넘긴다.
이게 끝나야 asmdef 에 **No Engine References** 를 켤 수 있다.

같이 처리할 것: `CardData` 가 public 필드투성이라 스타일 규칙 위반.

### 3. 카드 뽑기 장수를 상수 → 변수로
`Engine/Events/TurnStartEvent.cs` 가 `new DrawCardEvent(CharacterId, 1)` 로 고정.
"다음 턴 카드 +n" 같은 효과를 만들 때 두 개가 필요해진다:
- 기본 장수 → `CharacterData` → `CharacterState` → `CharacterRuntime`
- 일시 보정 → `StatusEffectRuntime` 이 이미 `OnTurnStart` 훅과 만료 처리를 갖고 있으므로 그쪽이 적합

### 4. 잔재 정리
- `UI/Slot/SpeedSlotUI.cs` — 참조 0건. `SlotDebugItem` 이 대체함
- `Engine/Battle/BattleInput.cs`, `BattleResult.cs`, `BattleRuntime.Start(BattleInput)` — 호출자 0.
  `Start` 내용은 `BattleManager.ExecuteCombat` 과 중복
- `TurnUI` 의 `endTurnButton` / `turnText` 필드 — 선언만 되고 쓰이지 않음

### 5. `Step()` 재귀 구조 (급하지 않음)
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
`Engine/` 에는 `using UnityEngine` 이 한 줄도 없다. 남은 누수는 위 "다음 작업 2".

## 현재 상태 (2026-07-22)

**전투 루프가 끝에서 끝까지 동작한다.** 씬 세팅 완료, 플레이로 확인됨.

```
전투 생성 → 턴 시작(속도 굴림 + 카드 1장 뽑기)
  → 슬롯 클릭 → 카드 드래그 → 상대 슬롯에 드롭 → 합 성립(BoutGraph edge)
    → 턴 종료 버튼 → 전투 해석 → 다음 턴
```

확인된 것: 합이 잡히면 양쪽 슬롯이 서로를 가리킨다(`Bout: 1-0` / `Bout: 0-0`).
**아직 확인 못 한 것: 데미지가 실제로 들어가는지.** 표시 UI 가 없어서.

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
- **상태 변경은 Event.Apply 안에서만.** `runtime.AddLog → CharacterRuntime 호출 → 후속 Event Enqueue` 순.
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
