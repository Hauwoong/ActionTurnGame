# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 다음 작업 (2026-07-30 갱신)

### ▶ 내일 시작 지점 — 카드 늘리기 (방어·회피·반격 주사위 확보)

**왜 이게 먼저인가.** 지금 카드가 `Strike`(Attack 주사위 1개) 하나뿐이라 **검증 부채가 4건 쌓여 있다.**
전부 "방어 주사위가 있어야" 또는 "주사위가 2개 이상이어야" 만들 수 있는 상황이다:

| 대기 중인 검증 | 필요한 것 |
|---|---|
| 3-1.5 — 방어·회피 주사위에서 출혈이 터지는가 | 방어 주사위 |
| 3-1.7 — 일방에서 방어 주사위가 **안 굴러야** 한다 | 방어 주사위 |
| 3-1.6 — 첫 루프 `DiceDiscardRemainingEvent` (대상 사망 후 잔여 소멸) | 주사위 2개 이상 |
| 3-1.6 — 둘째 루프 전체 + `DiscardRemaining` 의 보관 분기 | 양쪽 주사위 수가 다른 카드 |

카드를 늘리면 이 넷이 한꺼번에 열린다. 반대로 지금 다른 엔진 작업을 더 쌓으면 검증 부채도 같이 쌓인다.

**할 일 (Data 계층 위주, 엔진 변경 거의 없음)**
1. `Assets/Data/Cards/` 에 카드 에셋 추가. 최소 두 장:
   - **방어 주사위가 있는 카드** — `Block` 또는 `Evade` 를 포함. 위 4건 중 3건이 여기 걸린다
   - **주사위 2~3개짜리 공격 카드** — 다타 상황용
2. `CardData` 는 이미 `ToModel()` 구조라 코드 변경 불필요. 인스펙터에서 `DiceData` 를 채우는 작업.
3. 덱에 넣기 — `CharacterData` 의 덱 목록. `Ally01` / `Enemy01` 양쪽.
4. `BattleManager` 의 artwork 레지스트리는 이름 기반이라 새 카드도 자동으로 잡힌다(스프라이트만 지정).

**그다음 바로 검증** — 위 표의 4건을 순서대로. 3-1.5·3-1.7 은 대조군이 이미 기록돼 있으니 바로 비교된다.

**막히면**: `Strike.asset` 을 열어 필드 구조를 먼저 볼 것. `CardData` 는 `[SerializeField] private` +
프로퍼티 구조이고, 필드 리네임 이력이 있어 `FormerlySerializedAs` 가 붙어 있다(에셋 파일에 실제로 적힌 키 기준).

**대안 (카드 작업이 막히거나 내키지 않으면)**: 3-1.10(`DiceRecoverEvent` 커서 문제) — 단 그것도
관측하려면 방어 주사위가 필요하다. 아니면 "4. Engine → Data 의존 끊기 2단계"(`CharacterModel`)가
상태이상 작업과 완전히 독립적이라 머리를 쉬어가는 선택지.

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
- **3-2 Delay(발효 지연) 완료 (2026-07-27, 플레이 검증)** — DEVLOG `2026-07-27` 참고.
  **`Delay` 카운터도 `IsActive` 게이트도 래퍼 7개도 만들지 않았다.** 대기분을 따로 세기로 정하자
  (즉시 2 + 대기 3 → 이번 턴 +2, 다음 턴 +3) 그 셋이 통째로 불필요해졌다 — 대기분은 아무 훅도 안 읽는
  숫자일 뿐이라 "대기 중이라 훅을 막아야 하는 상태"가 존재하지 않는다. 복잡도는 `TickTurnEnd` 한 곳으로 모였다.
  - `Stack` / `PendingStack` 두 칸. `AddStack(int amount, bool delayed)` 이 분배를 독점.
  - `TickTurnEnd` 순서: `OnTurnEnd()` → duration 만료면 **활성분 버리기**(`Stack = 0`) →
    대기분 승격 + `Duration = _baseDuration` **재장전** → 그러고 나서 만료 판정. 순서 하나만 틀려도 깨진다.
  - `AddStatus` 는 "없거나 만료됐으면 `Create(type, this, 0)` + 등록 → 무조건 `AddStack`" 으로 두 분기를 합쳤다.
  - Bleed/Burn 은 진입 가드(`if (Stack <= 0) return;`)와 만료 조건(`&& PendingStack <= 0`)이 **둘 다** 필요하다.
    앞은 "진입할 때 0", 뒤는 "나가면서 0" 을 막는 서로 다른 순간이다.
- **3-1.7 / 3-1.8 / 3-1.9 완료 (2026-07-30, 플레이 검증)** — DEVLOG `2026-07-29 ~ 07-30` 참고.
  A안 채택. `DiceRollContext(Owner/Dice/ModifiedRoll)` 신설 + `OnModifyRoll` 훅을 상태이상·패시브 양쪽에 추가,
  `CharacterRuntime.TriggerModifyRoll(DiceRuntime) → int` 가 합·일방 두 경로의 공용 관문이 됐다.
  - **`isOwnerA` 인자가 통째로 소멸.** 훅이 자기 주사위 하나만 보게 되어 힘/마비/`AttackBoostPassive` 셋 다
    `if (isOwnerA) ... else ...` 대칭 코드가 한 줄로 줄었다. 훅 시그니처의 A/B 는 그릇이 크다는 신호였다.
  - **범위 규칙**: 힘 = 공격 주사위만(타입 가드 있음), 마비 = 전 타입. 효과마다 다르므로 타입 검사는 훅 안.
  - **굴림값 하한 1 은 `TriggerModifyRoll` 반환 직전 한 곳**(`Math.Max`, `Mathf` 아님).
    마비 안에 두면 순서 의존이고 감소 효과가 늘 때마다 재구현해야 한다. 원작이 "보정 합산 후 하한"이라
    결과도 이쪽이 맞다(굴림 3/마비 5/힘 2 → 마비 클램프 3, 래퍼 클램프 1). `TickTurnEnd` 와 같은 구도.
  - **보정값을 `DiceRuntime.CurrentRoll` 에 되쓰지 않는다.** "실제 굴린 값" 과 "이번 해석에서 쓸 값"은
    다른 개념이고, 합치면 "최대값 굴림 시 발동" 류를 만들 수 없다. `CurrentRoll` 은 `Roll()` 만 쓴다.
  - **`ResolveUnopposedDice` 는 이제 공격 주사위만 굴린다.** 방어·회피는 굴리지 않고 `DiceConsumedEvent` 만 낸다
    — 어차피 `Consume` 으로 보관됐다 나중에 다시 굴려지므로 지금 굴린 값은 버려지고, 3-1.5 가 넣은
    `TriggerDiceRoll` 때문에 버릴 굴림에 출혈만 터지고 있었다.
    원칙: **`Roll()` 이 있는 자리에 굴림 훅이 따라붙는다. 굴리지 않으면 훅도 없다.**
  - `ClashContext` 는 `DiceA`/`DiceB` 를 잃고 "두 캐릭터 + 두 숫자"만 남았다(타입 검사가 훅 안으로 갔으므로).
    `: IClashContext` 와 `Defender` 도 제거(3-1.8). `OnBeforeClash`/`TriggerBeforeClash` 3곳,
    `Refresh()` 삭제(3-1.9). `IsOwnerA` 대소문자 문제는 그 줄과 함께 소멸.
  - **실수 둘**: `ClashContext` 생성자 복붙(`ModifiedRollB = modifiedRollA` → 모든 합이 무승부),
    타입 검사를 위로 올리면서 주사위를 소비하지 않고 `return`(→ `ResolveCombat` 무한 루프).
    **`ResolveCombat` 의 `while` 에서 "아무것도 안 하고 빠지기"는 존재하지 않는다** — 모든 탈출 경로가
    `DiceConsumed`/`DiceDestroyed` 중 하나를 내야 한다. `RunQueue` 의 `visited.Add(slot)` 과 같은 구조.
- **3-1.6 완료 + 주사위 수명 정리 (2026-07-30, 플레이 검증)** — DEVLOG `2026-07-30` 참고.
  **원작 규칙 확정(사용자): 죽으면 그 캐릭터의 남은 주사위는 소멸. 대상이 죽으면 공격자의 남은 주사위도 소멸(A안).**
  - **가드는 `DamageEvent`/`StaggerEvent` 맨 앞의 `Attacker.IsDead`** — 합·일방 두 경로가 반드시 지나는
    가장 깊은 지점. `ClashContextEvent` 에 두면 합만 덮여서 3-1.7 에서 합친 걸 다시 가르는 꼴이 된다.
    `TakeDamage` 의 `if (_isDead) return;` 과 대칭짝(죽은 방어자는 이미 막혀 있었고 공격자만 빠져 있었다).
  - `ResolveUnopposedDice` 의 early return 은 **낭비 제거용**이지 정확성 담당이 아니다.
  - **합에서는 명시적으로 중단하지 않는다.** `ToAdvanceEvent` 가 그대로 돌아야 상대 주사위가 큐에서 빠진다.
    `rule.Resolve` 앞에서 `return` 하면 B 주사위가 `Ready` 로 남아 둘째 루프에서 재굴림된다.
  - **`DicePool` 이 범위 기준으로 통일됐다**: `DestroyRemaining`(커서→끝, 전부 — **죽음**) /
    `DiscardRemaining`(커서→끝, Attack 만 소멸·나머지는 `Consume` 보관 — **중단**). `Clear` 어법과
    `DestroyAllDice` 삭제. 커서를 `_dice.Count` 로 미는 게 필수 — 안 밀면 `Inject` 가 소멸된 주사위 앞에 꽂힌다.
  - **`Advance` 경계 가드** — `Peek` 이 null 을 준 뒤에도 `DiceDestroyedEvent` 가 `AdvanceDice` 를 무조건 부른다.
    이 가드가 3-1.6 을 "크래시"에서 "관찰 가능한 버그"로 바꿔줬다.
  - **합·일방 순서 통일**: 확보 → `Roll` → `TriggerModifyRoll` → `TriggerDiceRoll` → 죽음 확인 → 해석 → 로그 → 이벤트.
    `TriggerDiceRoll` 이 합에선 `rule.Resolve` 뒤에 있던 건 3-1.5 때 개명만 하고 위치를 안 옮긴 흔적.
  - `IsTargetAlive(SpeedSlot)` → `IsAlive(int)` 로 교체, `RunQueue` 까지 통일.
  - **`_cursor` 에 -1 을 넣어 죽음 신호로 쓰지 말 것** — 죽음은 `_isDead` 가 권위다. "위치" 필드에 "상태"를
    겸하게 하는 건 3-1.10 의 `Ready` 이중 의미와 같은 병이고, `Peek`/`Inject` 가 곧장 예외를 던진다.
  - **검증 대기 (카드가 `Strike` 하나뿐이라 상황을 만들 수 없음)**: 첫 루프의 `DiceDiscardRemainingEvent`,
    둘째 루프 전체(`Attack vs Attack` 이 `destroyBoth` 라 양쪽이 동시에 바닥난다),
    `DiscardRemaining` 의 방어 주사위 보관 분기.

### 3. 상태이상 — 남은 것

- **3-1.10. `DiceRecoverEvent` 가 실제로는 효과가 없어 보인다 (2026-07-30 발견, 미확인)**
  의도는 "일방에서 굴리지 않고 보관한 방어·회피 주사위를 나중에 꺼내 쓴다"인데 커서가 막고 있다:
  - `DicePool.Advance(Consume)` 가 `_cursor++` 를 하는데 `Recover()` 는 **상태만** 되돌리고 커서는 안 건드린다
  - `Peek()` 은 `_cursor` 부터 **앞으로만** 훑고, `Inject` 는 새 주사위를 `_cursor` **위치에** 꽂는다
  - 그래서 보관된 주사위는 `Ready` 로 돌아와도 그 턴 안에 다시 잡히지 않고,
    `ResetForNextTurn` 이 `_cursor = 0` 과 함께 전부 `Destroy` 한다
  - 3-1.6 과 같이 볼 것. 둘 다 `ResolveCombat`/`DicePool` 의 주사위 수명 문제다.
- **3-1.11. `DiceClashLog` 가 보정 전 `CurrentRoll` 을 찍는다** — 합에서 검증할 때 로그 굴림값과
  데미지가 어긋나 보인다. `clashCtx.ModifiedRollA/B` 로 바꾸거나 원본과 보정값을 둘 다 싣는 선택지.
  일부러 안 고쳤다 — 3-1.7 검증 중에 바꾸면 "훅이 도는 것"과 "로그가 바뀐 것"을 구분할 수 없어서.
- **3-3. 훅 순회를 스냅샷 → 재사용 없는 인덱스 루프로** — `ToArray()` 할당 제거.
  **공유 버퍼 필드 하나로 돌려쓰면 재진입 때문에 깨진다**(바깥 루프가 쓰던 배열을 안쪽이 덮어씀).
  올바른 방향은 `_triggerDepth` 카운터로 "순회 중엔 제거·정렬 금지"를 강제하는 것:
  - `FlushExpired` / `EnsureSorted` 맨 앞에 `if (_triggerDepth > 0) return;` (`EnsureSorted` 는 `_dirty` 를 내리지 않는다)
  - 호출 순서가 함정: `EnsureSorted()` → `_triggerDepth++` → `try { 인덱스 루프 } finally { _triggerDepth--; }` → `FlushExpired()`.
    `EnsureSorted` 를 `++` 뒤에 두면 최외곽도 정렬을 건너뛰고, `FlushExpired` 를 `finally` 안에 두면 영영 안 지워진다
  - `finally` 필수 — 예외로 depth 가 안 내려가면 그 캐릭터는 이후 전투 내내 정렬·만료가 멈춘다
  - 실익은 성능이 아니라 구조다(리스트가 비어 있어 지금은 할당도 거의 없음). 우선순위 낮음
- ~~**3-4. 상태이상 UI**~~ — **완료 (2026-07-27, 플레이 검증).** `StatusUI.cs` 신설.
  `TMP_Text` 한 줄에 `Bleed 5 | Strength 0 (+3)` 식으로 나열, 비면 `-`(빈 문자열이면
  "효과 없음"과 "UI 가 안 돎"이 구분 안 되므로). 이제 상태이상 검증에 브레이크포인트가 필요 없다.
  - **갱신 시점이 이 작업의 핵심이었다.** 상태이상 변경 중 자기 로그를 가진 건 부여(`StatusAddLog`)뿐이고
    스택 절반 감소·만료·대기분 승격·duration 감소는 로그가 없다. **새 로그를 만들지 않고**,
    그 변경들이 이미 다른 로그와 같은 순간에 일어난다는 점을 이용했다:
    스택 감소는 `StatusDamageLog` 와 같은 자리, 나머지는 전부 `TriggerTurnEnd` 안이고
    `TurnEndLog` 가 그 뒤에 나간다. 구독은 `StatusAdd`/`StatusDamage`/`TurnEnd`/`TurnStart` 4개.
  - **남는 구멍**: 데미지를 안 내는 효과가 예상 밖 순간에 바뀌면 놓친다. 새 효과를 만들 때
    "이건 어느 로그에 얹혀 가나?" 를 한 번 생각할 것.
  - `Refresh` 는 `IsExpired` 를 건너뛴다 — `FlushExpired` 는 훅 루프 뒤에 도는데 그 사이에 로그가 나간다.
  - 노출은 `public IReadOnlyList<StatusEffectRuntime> StatusEffects`. **원소는 여전히 가변**이고
    `AddStack` 이 public 이라 UI 가 부를 수 있다(`SpeedSlots` 도 동일). 막으려면 읽기 전용 view 타입이
    필요한데, 그건 `SpeedSlots` 까지 같이 바꿔야 의미가 있다.

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
- `SlotDebugPanel.cs:57` 의 진단용 `Debug.Log("[BoutStart] ...")` — 매 bout 마다 스택 트레이스까지
  찍힌다. 2026-07-22 의 `HpUI` 건과 같은 종류(그때는 필터 앞에 있어서 "데미지 로그 2번" 오해를 낳았다)
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
