# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 다음 작업 (2026-08-22 갱신)

### ▶ 다음 시작 지점 — **봉인 효과** (아래 9번 "봉인 규칙 확정")

**작업 순서를 바꿨다 (2026-08-22 사용자 결정).** `CharacterRuntime` 3단계까지 끝났고 남은 것은
4단계(메서드 38개 `///` 주석)인데, **그걸 뒤로 미룬다.**

```
1. 슬롯 UI 속도순 정렬        ✔ 완료 (2026-08-22, 플레이 검증)
2. 봉인 효과                  ← 여기부터. 설계 결정 3개와 에셋이 먼저 필요
3. 주석 38개 + 봉인이 추가한 메서드   (최종 형태에 대해 한 번)
```

**1번에 딸려 나온 것들도 같은 날 끝났다 (2026-08-22)** — 전부 봉인 작업의 걸림돌이던 것들이다:
- **`_slotRuntimeMap` 버그** — 고친 게 아니라 **맵을 없앴다.** 조회가 계산 가능했다(9번 참고).
  봉인 때 처음 터질 자리였는데 미리 사라졌다
- **`ISpeedLookup`** — `BoutGraph` 가 속도라는 `int` 하나 말고는 슬롯에 손이 안 닿는다

**2번을 시작하기 전에 정해야 할 것 (2026-08-22 "다음 턴 적용" 규칙으로 셋 → 하나로 줄었다)**
- ~~**턴 중에 봉인되면 이미 등록된 액션은?**~~ — **해소.** 이번 턴은 정상이므로
  **등록된 액션은 그대로 수행된다.** 해석 도중 취소를 다룰 필요가 없다
- ~~**발동 조건이 착수 전제**~~ — **아니게 됐다.** 지연 적용이라 `DebugAddBleed` 류의
  **임시 버튼 하나로 검증이 된다**(3-1 검증 때 쓴 그 패턴). 진짜 발동 조건(주사위/카드)은
  나중에 배선해도 되고, 그때 **엔진은 이미 검증된 상태**다
- **남은 것 — 지속시간.** 한 번 걸린 봉인이 몇 턴 가는가. 상태이상이면
  `StatusEffectRuntime` 의 duration 기계가 그대로 받는다

**이유: 봉인은 `CharacterRuntime` 에 메서드를 더한다**(봉인 걸기 / "가장 빠른 슬롯" 조회).
지금 38개에 주석을 달면 새 메서드에 또 달아야 하고 속도 슬롯 쪽은 다시 쓰게 된다.
**"목록도 코드처럼 썩는다"**(2026-08-21)의 주석판이다 — 곧 고칠 코드에 다는 문서는 쓰자마자 헌다.

✔ **4번(Engine → Data 의존 끊기)이 통째로 끝났다 (2026-08-21, 플레이 검증).**
asmdef 로 게이트가 섰고 **이제 컴파일러가 규칙을 지킨다.** 자세한 것은 아래 4번 항목.

**그래서 "엔진이 일단락되면 주석 작업" 조건이 충족됐고, 주석 작업을 시작했다 (2026-08-21).**

#### 주석 작업 진행 상황

| | 대상 | 상태 |
|---|---|---|
| 1 | **거짓이 된 주석 4줄** (`CharacterState`/`BattleSnapShot`×2/`BattleManager`) | ✔ 완료 |
| 2 | **`DicePool.cs`** — 클래스 요약 + 메서드 9개 `///` + 불변량 3건 | ✔ 완료 |
| 3 | **`CombatExecutor.ResolveUnopposedDice`** — 저장 규칙 짝 주석 + 죽음 가드 근거 | ✔ 완료 |
| 4 | **`CharacterRuntime.cs`** — 아래 재개 절차 | **1~3단계 완료, 4단계(주석)는 봉인 뒤로 미룸** |

**1~3 으로 CLAUDE.md 의 "안 달린 주석" 목록이 전부 소진됐다.**
`CombatExecutor.cs:162` / `DicePool.cs:81` 의 "여기 `Attack` 만인 건 저장 규칙 때문" 도 들어갔다.

#### ▶▶ 재개 절차 — `CharacterRuntime.cs` (메서드 38개)

**각 단계를 따로 커밋한다.** 3단계의 diff 가 "순수 이동" 이어야 대조가 의미를 갖기 때문이다.

1. ✔ **완료** — `EmotionLevelUp` 상한 가드 + 죽은 필드 2개(`_MaxEmotionLevel`/`_maxEmotionStack`) 삭제
2. ✔ **완료 (2026-08-22, 플레이 검증)** — **개수 배선을 했다가 되돌리고 개명만 남겼다.**
   착수한 다음날 사용자가 원작에서 봉인 규칙을 확인했고, 개수(`int`) 모델로는 표현이 안 되는 게
   드러났다. 자세한 것은 9번 항목의 **"봉인 규칙 확정"** 절. 남은 결과물:
   - `SpeedSlots` → **`SpeedSlotPool`** 개명 (3곳). 봉인은 `IsSealed` 플래그로 갈 것이고,
     그때도 이 이름이 `foreach` 로 봉인 슬롯을 조용히 포함시키는 걸 막아준다
   - `_activeSpeedSlotCount` **삭제** — 9번 표의 "쓰기 전용" 항목이 배선이 아니라 제거로 닫혔다.
     한 필드가 **"슬롯을 몇 개 가졌나"(개수)** 와 **"이번 턴에 쓰나"(플래그)** 두 축을 겸하고 있었다
   - `SetSpeedSlotCount` → **`EnsureSpeedSlotCount`** 개명 + `CreateSpeedSlots` 를 그 안으로 접음.
     몸통이 `while` 하나만 남아 이름이 하는 일과 어긋났다(`ReturnCard`/`EndBout` 때와 같은 판단).
     `EnsureSorted` 와 같은 접두사·같은 성격(멱등)이라 어휘를 새로 만들지 않았다
   - **덤: 어제 걱정한 "절대값 setter라 합성이 안 된다" 가 이걸로 해소됐다.** 아래 그 블록 참고
3. **카테고리 재정렬** (순수 이동, 주석 금지). 배너는 `BattleRuntime` 스타일
   (`// ──────────── 이름 ────────────`). 목표 순서:
   상태(필드) → 조회(프로퍼티) → 생성자 → 이벤트 → HP·죽음 → 스태거 → 에너지(빛) → 감정 →
   속도 슬롯 → 카드·액션 → 주사위 → 상태이상 → Trigger 훅 → private 헬퍼
   - **실제로 옮겨지는 건 셋뿐이다**: `ExitStagger`(스태거 그룹으로) /
     `EnsureSpeedSlotCount`(감정 구간에서 빼냄) / `UseEnergy`(`CanUseAction`·`UseAction` 사이에서 빼냄)
   - **Trigger 8개는 파이프라인 순서로**: `TurnStart → ModifyRoll → DiceRoll → BeforeDamage →
     AfterDamage → BeforeStagger → AfterStagger → TurnEnd`. 제일 큰 이득은 `TurnEnd` 가 한복판에서 끝으로 가는 것
   - **검증: 이동 전후로 메서드 38개, 이름이 전부 일치해야 한다.** 이동 전 목록(정렬,
     2026-08-22 2단계 반영):
     `AddStatus` `Advance` `CanUseAction` `ChangeMaxEnergy` `ChangeMaxHp` `ChangeMaxStagger`
     `CharacterRuntime`(생성자) `DestroyRemainingDice` `Die` `DiscardRemainingDice`
     `EmotionLevelUp` `EndBoutDice` `EnqueueEvent` `EnsureSorted` `EnsureSpeedSlotCount`
     `EnterStagger` `ExitStagger`
     `FlushExpired` `GainEmotionStack` `GetDiceInfo` `Peek` `RecoverEnergy` `RecoverStagger`
     `ResetDiceForNextTurn` `ShouldDie` `ShouldEnterStagger` `TakeDamage`
     `TakeStagger` `TriggerAfterDamage` `TriggerAfterStagger` `TriggerBeforeDamage`
     `TriggerBeforeStagger` `TriggerDiceRoll` `TriggerModifyRoll` `TriggerTurnEnd` `TriggerTurnStart`
     `UseAction` `UseEnergy`
     - **2단계가 이 목록을 바꿨다**: `CreateSpeedSlots` 삭제(-1), `SetSpeedSlotCount` →
       `EnsureSpeedSlotCount` 개명. 39 → **38개**. `SpeedSlotPool` 개명은 프로퍼티라 목록 밖이다
4. **메서드 38개 `///` 주석.** 카테고리 순서대로

#### 주석 작업에서 반복된 것 — 다음에 먼저 볼 것

- **오타가 하필 타입·메서드 이름에 난다.** 이번에 `Chracter`×3 / `Infect`(→`Inject`) /
  `envade`(→`Evade`) / `멱동`(→`멱등`) 이 났다. **`CharacterData` 를 grep 해도 그 줄들이 안 잡힌다** —
  오늘 "거짓이 된 주석 목록이 헐어 있었다" 를 발견한 방법이 바로 grep 이었다
- **목록을 표로 나누면 표 하나를 통째로 건너뛴다.** 식별자 오타표는 다 고치고 용어 오타표는
  하나도 안 고쳐진 적이 있다. "짝인데 한쪽만" 의 문서판

**2026-08-06 에 1~2번(`BoutStart`·`BoutEndLog` 합 여부 / `DamageLog`·`StaggerLog` 공격자)과
`IsOffensive` 를 끝냈고, 2026-08-19 에 6번 잔재 정리도 끝냈다.**
주석 작업 뒤에 남는 것은 **8번 3·5번 항목(적 AI 와 함께)** / **3-3 훅 순회** /
**5번 드로우 장수 변수화** / **10번 UI(전투 해석 연출)** 이고, 큰 덩어리는 없다.

2026-08-05~06 세션에서 끝난 것: **3-1.10 (1~5단계 전부)**, **3-1.12**, **3-1.11 + `UnopposedLog` 배선**,
**3-1.13 (힘/패시브가 `Counter` 를 안 올려주던 것) + `IsOffensive` 까지 완결**,
그리고 검증 계기인 **전역 로그 콘솔 `DiceUI`**.
`_diceById` 도 첫 독자(`GetDiceInfo`)가 생겨 "쓰기 전용" 상태가 끝났다.

**사용자 결정 (2026-08-06): `IsOffensive` → 1+2번 순으로 간다.** `IsOffensive` 는 완료.

1. ~~**`BoutStartLog` / `BoutEndLog` 에 합 여부**~~ — ✔ **완료 (2026-08-06, 플레이 검증).**
   아래 "완료" 목록 참고
2. ~~**`DamageLog` / `StaggerLog` 에 공격자**~~ — ✔ **완료 (2026-08-06, 플레이 검증).**
   아래 "완료" 목록 참고
3. ~~**4번 2단계 — `CharacterModel` 신설**~~ — ✔ **완료 (2026-08-20, 플레이 검증).**
   3단계를 먼저 한 덕에 기계적인 작업이 됐다. 아래 4번 항목 참고

주석 작업에 넣을 것은 아래 두 목록이다. `CombatExecutor.cs:162` / `DicePool.cs:81` 의
"여기 `Attack` 만인 건 저장 규칙 때문"도 그때 같이.

**⚠ 먼저 볼 것 — 거짓이 된 주석들 (2026-08-21 실측으로 갱신).** 안 달린 게 아니라 **틀렸다.**
`CharacterState` 는 이제 카드·패시브 변환을 하지 않는다(그 일은 `CharacterData.ToModel()` 로 갔다).
**남은 것은 3파일 4줄이다:**
- `CharacterState.cs:3` 파일 머리 — "`CharacterData`(청사진)로 한 번 빌드되고".
  새 설명은 "**모델을 받아 패시브를 적용해 얼린다**" 가 맞다
- `BattleSnapShot.cs:4` — "**총 3계층(`CharacterData` → `CharacterState` → `CharacterRuntime`)**".
  계층 수도 첫 타입도 틀렸다. **4계층**이다:
  `CharacterData`(SO 청사진) → `CharacterModel`(순수 청사진) → `CharacterState`(스냅샷) → `CharacterRuntime`(가변)
- `BattleSnapShot.cs:16` — "양 진영 `CharacterData` 를 …" → `CharacterModel`
- `BattleManager.cs:34` XML — "`Character` → `CharacterData`(Asset) 로 즉시 평가" → `CharacterModel`.
  **`Engine/` 밖(Scene)이라 asmdef 게이트에 안 걸린다** — 컴파일러가 안 잡아주는 유일한 항목

- ~~`CharacterState.cs:26~27` XML~~ — **이미 고쳐져 있다**(2026-08-20 작업 중 같이 손댐).
  `CharacterStateBuilder.cs` `<param>` 의 "청사진" 도 지금은 모델을 가리켜 **거짓이 아니다**.
  **목록을 만든 뒤 손댄 것이 목록에 반영되지 않았다 — 목록도 코드처럼 썩는다**

그때 같이 넣을 것:
- `EndBout` 위 — "**멱등이어야 한다**"(합에서 방어자가 곧 `TargetId` 라 `BoutEndEvent` 가
  같은 캐릭터를 두 번 부르는 것에 기대고 있다)
- `Inject` 위 — "**bout 시작에 `_cursor == 0`** 이라는 불변량에 의존"(깨지면 새 주사위가
  리스트 중간에 꽂히는데, 증상이 "새 주사위가 저장분 뒤에 실림"으로 보여 원인을 엉뚱한 데서 찾게 된다)
- `EndBout` 의 `// 한 교전 끝` → 이 프로젝트 용어는 bout = 합

#### 진행 상황

| 단계 | 내용 | 상태 |
|---|---|---|
| 1 | `DiceState.Stored` + `Peek` 확장 + `DiceRuntime.Store()` | ✔ **검증 완료** |
| 2 | `DicePool.EndBout()` / `BoutEndEvent` 직접 호출 / 이벤트 4개 삭제 | ✔ **검증 완료** |
| 3 | `Inject(List<DiceEntry>)` = `InsertRange(_cursor, ...)`, `Add` 삭제 | ✔ **검증 완료** |
| 4 | `ResolveCombat` 두 루프를 **`targetId` 기준으로** (`a`/`b`/`idB` 소멸) | ✔ **검증 완료** |
| 5 | ~~`Used` 를 턴 끝 소멸로~~ → **`Reuse` 를 이벤트로 배선** + `DestroyUsed()` 삭제 | ✔ **검증 완료** |

**3-1.10 종료.** 3-1.12(파괴분을 `_dice` 에 안 쌓기)도 2단계의 `ClearDestroyed()` 로 함께 끝났다.
남은 파생 항목은 **`_diceById` 읽는 배선 0건** 하나다.

`_diceById` 채우는 루프는 복구돼 있다(`CharacterRuntime.cs:207`). 읽는 배선은 아직 0건이고
용도는 3-1.12 에서 확정한 대로 "불변 속성 조회용 대장"이다.

#### 4단계 — ✔ **완료·검증 완료 (2026-08-05)**. 아래는 설계 근거 기록

DEVLOG `2026-08-05 (후속)` 참고. **구현된 형태가 아래 그대로다.**

**`ResolveCombat` 의 두 루프를 전부 `targetId` 기준으로 바꾸는 것이었다.
`ResolveUnopposedDice` 는 한 글자도 안 고쳤다.**

```
ResolveCombat(int attackerId, int targetId)     // a / b / idB 전부 소멸

  첫 루프    diceB = Peek(targetId)                     // 조건 없음
             diceB != null ? Clash : Unopposed
  둘째 루프  while (Peek(targetId) != null) → Unopposed(targetId, attackerId)
             ( !IsAlive(attackerId) 이면 Discard 하고 break — 지금 그대로 )
```

- **`a` 는 원래부터 본문에서 한 번도 안 쓰였다.** `idA` 도 `a.SourceSlot.CharacterId` 와 같은
  값을 따로 받은 중복이다
- **`idB` 는 잉여다.** `RunQueue` 가 `opponent = ActionBySlot[targetSlot]` 로 잡으므로
  `opponent.SourceSlot == targetSlot` 이고, 따라서 **`b != null` 이면 `idB == targetId` 가 항상 참**이다.
  `idB`(`int?`)가 나르던 정보는 캐릭터가 아니라 **"상대가 bout 참가자인가"라는 bool** 이었다.
  일방이면 `idB` 만 null 이 되고 `targetId` 는 멀쩡히 살아 있다 — 그게 지금 대상 풀을 못 보는 이유
- 호출부는 `ResolveCombat(AttackerId, TargetId)`. `BoutStartEvent` 는 `UseAction(B)` 와
  `BoutEndEvent` 에 넘길 id 때문에 `B` 를 계속 들고 있으면 된다

**초판의 아래 두 항목은 틀렸다 (2026-08-05 사용자 교정).** 둘 다 "비대칭이 실제 규칙 차이다"라고
적었는데, 확인해보니 **규칙 차이가 아니다.** 대칭으로 가는 게 맞다.

- ~~"둘째 루프는 `b != null` 게이트를 유지한다. 아니면 일방으로 맞은 사람이 반격한다"~~ —
  **반격은 안 나온다.** 저장되는 건 정의상 방어 주사위뿐이고(공격 주사위는 상대가 없으면
  `DicePool.cs:87` 의 `DiscardRemaining` 이 파괴한다), 남은 저장분은 `ResolveUnopposedDice` 의
  비-Attack 분기로 가서 `Consume` → `EndBout` 이 다시 `Store` 한다. **왕복해서 제자리다.**
  `DiceRuntime` 의 상태 setter 에 가드가 없어(`DiceRuntime.cs:19~21`) `Stored → Consumed → Stored`
  전이도 안전하다. 실질 차이는 `DiceConsumedLog` 노이즈 한 줄뿐
- ~~"일방이면 대상의 저장분은 **공격 주사위**를 맞을 때만 나온다"~~ — **규칙은 "맞붙을 상대가
  있는가"지 "공격 주사위인가"가 아니다.** 대상 풀에 주사위가 있으면 그 순간 일방이 아니므로
  때리는 쪽의 방어 주사위도 굴린다. 3-1.9 가 정한 것도 "굴릴 상대가 없으면 저장"이었다.
  → **`ResolveUnopposedDice` 의 `if (Type != Attack)` 을 루프로 끌어올리면 안 된다.**
  그대로 두면 그 자리가 이제 진짜로 "대상 풀이 비었을 때"가 되어 의미까지 맞아떨어진다

**혼동의 원인은 항목 제목이었다** — "`ResolveUnopposedDice` 가 대상 풀 peek" 이라고 적어놔서
그 메서드에 뭘 다는 작업처럼 읽혔다. 실제로는 **호출자가 대상 풀을 보게 되면 그 메서드는
자동으로 옳아진다.** 표의 4단계 이름도 그래서 바꿨다.

**무한 루프는 안전하다.** 둘째 루프를 대칭으로 열어도 비-Attack 은 `Consume`, Attack 은
`Destroy` 로 매번 커서가 전진하고, 룰 테이블에 `(Reuse, Reuse)` 조합이 없다
(Counter vs Counter 도 `(Reuse, Destroy)`). "`ResolveCombat` 의 while 에서 아무것도 안 하고
빠지는 경로는 없다"는 기존 규칙 유지.

**`PeekForDefence` 같은 걸 따로 만들 필요가 없다.** 대상 풀에서 나올 수 있는 건 `Stored` 뿐이다:
- 주사위는 **bout 이 시작할 때** 실린다(`BoutStartEvent.cs:20~28` 의 `UseAction`). 액션 등록
  시점이 아니다. 그래서 **아직 bout 을 안 치른 캐릭터의 풀은 비어 있다**
- bout 은 DFS 로 하나씩 동기 처리되므로 **남의 bout 이 도는 동안 내 풀에 `Ready` 가 있을 수 없다**
- bout 이 끝날 때도 `Ready` 가 안 남는다 — 모든 탈출 경로가 커서를 끝까지 민다
  (정상 소진 / 대상 사망 → `DiscardRemaining` / 본인 사망 → `DestroyRemaining` /
  `IsValidAction` false → bout 자체가 안 생겨 적재도 없음). 거기에 `EndBout` 이 정리를 끝낸다

**새 불변량 — "bout 종료 시 커서는 항상 끝에 있다".** 위 성질이 4단계의 근거가 되는 순간
이건 **지켜야 할 규칙**이 된다(지금은 결과적으로 성립할 뿐 강제되지 않는다). 깨지면 남의 bout 에서
내 `Ready` 공격 주사위가 방어에 끌려 나오고, **증상은 "자기 차례에 주사위가 모자람"으로 보여
원인을 엉뚱한 데서 찾게 된다.** `ResolveCombat` 의 while 에서 "아무것도 안 하고 빠지기"가
없어야 한다는 기존 규칙과 같은 종류다. "bout 밖에서는 모든 커서가 0" 과 짝이다.

**구조 메모 — A 중심인 건 큐 때문이다.** `RunQueue` 가 속도순으로 액션을 하나 뽑고
뽑힌 쪽이 A, 맞는 쪽이 target 이 된다. 대등한 관계가 아니라 "누가 먼저 뽑혔나"가 역할을 정한다.
그래서 루프를 끝내는 것도 A 뿐이고(`if (diceA == null) break;`) 둘째 루프는 대상의 뒤처리 부록이다.
**주사위를 해석하는 층(`ResolveDiceClash`)은 완전 대칭이다** — 3-1.7 이 `isOwnerA` 를 없앤 결과다.
4단계 뒤에는 **주사위를 꺼내는 층도 대칭이 된다**(양쪽 다 `Peek(id)`). 남는 비대칭은
"누가 bout 을 주도하나" 하나뿐이고, 그건 `ResolveDiceClash` 는 물론 `ResolveCombat` 도 안 읽는다.

**4단계 검증 레시피 — 실시 완료, 통과** (출혈 = 굴림 카운터, 배선 0. Ally 5/5 · Enemy 1/1 이라 아군 슬롯이 항상 먼저)
1. **Ally 슬롯0** = `Guard` → **비어 있는 적 슬롯** 지목 (합이 되면 안 된다. `Bout: -` 확인)
2. **Enemy 슬롯0** = `Strike` → **비어 있는 Ally 슬롯1** 지목
3. Ally 에 출혈 5 (`DebugAddBleed`)

bout1 에서 Ally 의 Block 이 저장되고, bout2 에서 Enemy 가 Ally 를 일방으로 때린다.
- **수정 전**: Ally 는 한 번도 안 굴린다 → 출혈 **5 유지**
- **수정 후**: 저장 Block 이 방어로 굴러간다 → 출혈 **5→2**. 데미지도 `공격값 - 수비값` 으로 줄어야 한다

두 가설이 서로 다른 숫자를 예측한다 — A 검증 초판이 "양쪽 다 0 을 예측"해서 무정보였던 것과 대비.

2단계에서 이렇게 됐다: `DiceRecoverEvent`/`DiceRecoverLog`/`DiceDestroyUsedEvent`/`DiceDestroyUsedLog`
**삭제**, `CharacterRuntime` 은 `EndBoutDice()` 하나만 노출, `DicePool` 의 `StoreConsumed`/
`DestroyUsed`/`ClearDestroyed` 는 `private`, `ResetForNextTurn` 은 `_dice.Clear()` + `_cursor = 0`.
(5단계에서 `DestroyUsed` 는 삭제되고 `StoreConsumed` 는 `StoreSurvivors` 로 바뀌었다 — `Consumed` 와 `Used` 를 둘 다 `Stored` 로.)
`BoutEndEvent` 는 `{AttackerId, TargetId}` 만 정리한다(`DefenderId` 는 항상 `TargetId` 와
같거나 null 이므로 완전하다 — 5대5 에서도 참).

#### 5단계 — ✔ **완료·검증 완료 (2026-08-06)**. `Reuse` 배선 (**원래 계획서와 내용이 다르다**)

DEVLOG `2026-08-06` 참고. **아래 "할 일" 4개가 그대로 구현됐다.**

**발견: `DiceState.Used` 는 도달 불가 상태다.**

```
DiceRuleTable        Reuse 를 8곳에서 생산 (Counter/Evade 승리)
  ↓
ToAdvanceEvent       AdvanceType.Reuse => null        ← CombatExecutor.cs:201
  ↓
(이벤트 없음)         DicePool.Advance(Reuse) 호출자 0건
  ↓
DiceRuntime.Use()    호출자 0건 → DiceState.Used 가 절대 안 찍힌다
```

따라서 **`DestroyUsed()` / `Peek` 의 `state == Used` 검사 / `Advance` 의 `Reuse` 케이스 /
`Use()` 가 전부 도달 불가**다. "지금은 합 끝에 소멸한다"는 계획서의 전제가 성립한 적이 없다.

**왜 안 보였나 — `Reuse` 는 "커서를 안 움직인다"가 전부인데, 아무것도 안 하면 그게 달성된다.**
주사위가 `Ready` 로 남고 `Peek` 이 다음 iteration 에서 같은 걸 다시 집는다.
**반격 재굴림은 실제로 동작한다.** 상태 라벨만 안 붙는다. 그래서 지금 반격 주사위는
결국 `Consume` → `EndBout` 이 `Store` → **턴 내내 저장분으로 살아남고, 원작 규칙과 이미 일치한다.**

**결정 (사용자, 2026-08-05): 배선하는 쪽으로 간다.**

- **근거는 로그가 아니다.** 재사용 횟수는 이미 `DiceClashLog` 의 `advanceA`/`advanceB` 에 있고
  (`CombatExecutor.cs:145~149`), `CombatLogs` 에서 `DiceHandle` 별로 세면 나온다.
  **`State` 는 덮어써지는 필드라 횟수를 못 센다** — "가변 객체는 이력이 아니다"(2026-07-30 결론)
- **진짜 근거는 `Reuse` 만 이벤트를 안 내는 예외라는 것.** 이 프로젝트 규칙
  "`ResolveCombat` 의 while 에서 아무것도 안 하고 빠지기는 존재하지 않는다"의 유일한 예외이고,
  지금 안전한 건 "주사위가 커서에 남아 다음 iteration 이 다시 처리한다"는 **암묵 성질** 덕이다.
  4단계 버그도 암묵 불변량("합에 안 낀 캐릭터의 풀은 비어 있다")이 깨져서 난 것이다

**할 일**

1. `ToAdvanceEvent` 의 `AdvanceType.Reuse => null` → `new DiceReuseEvent(characterId)`.
   `Apply` 는 `AdvanceDice(id, Reuse)` + `DiceReuseLog`. **커서는 안 움직인다**
   (`DicePool.Advance` 가 이미 그렇게 돼 있다 — `Use()` 만 하고 `_cursor++` 없음)
2. **`DestroyUsed()` 삭제.** 죽은 코드일 뿐 아니라 **규칙상으로도 틀렸다** — 살아나면
   반격 주사위를 bout 끝에 죽여서 "턴 끝까지 산다"를 어긴다
3. `EndBout` 이 `Consumed` 와 `Used` 를 **둘 다 `Stored`** 로. 안 그러면 bout 밖에서
   `Used` 와 `Stored` 가 같은 뜻이 되어 **라벨 두 개가 한 뜻**을 갖는다
   (`Ready` 가 두 뜻을 겸했던 것의 반대 방향 같은 병)
4. `Peek` 의 `Used` 검사는 **유지** — bout 안에서 다시 집어야 한다

**접은 대안(A안): `Used` 를 통째로 삭제.** enum 값 / `Use()` / `Advance` 의 `Reuse` 케이스 /
`Peek` 의 `Used` 검사까지. 더 작고 실제 동작과 일치하지만, `Reuse` 만 이벤트 경로 밖에 남는다.

**검증 — 관측 가능한 동작 변화가 0이다. 이걸 먼저 알고 시작할 것.**

- 배선 전: `Reuse` → 아무 일 없음 → 주사위가 `Ready` 로 커서에 남음 → `Peek` 이 다시 집음
- 배선 후: `Reuse` → `Used` → 주사위가 `Used` 로 커서에 남음 → `Peek` 이 다시 집음

**재굴림 횟수도 데미지도 같다.** 출혈 카운터로는 절대 안 갈린다. 그러니 검증은
**`DiceReuseEvent.Apply` 브레이크포인트가 걸리는가** 하나이고, 나머지는 회귀 확인이다.
(브레이크포인트를 쓸 땐 Code Optimization 이 Release 면 거짓 신호가 난다 — 이미 두 번 겪었다)

**`EndBout` 의 `Used → Stored`(3번)는 현재 도달 불가다.** `Reuse` 된 주사위는 커서에 남아
모든 탈출 경로가 다시 집어가므로 bout 끝에는 이미 `Consumed`/`Destroyed` 다
(첫 루프는 `Peek` 이 `Used` 를 돌려주니 안 끊기고, 둘째 루프·`DiscardRemaining` 은 비-Attack 이라
`Consume`, 죽으면 `DestroyRemaining`). **그래서 3번은 검증 대상이 아니라 보험이다.**
- `Consumed`/`Used` 를 명시적으로 나열할 것. "`Destroyed` 아니면 전부 `Stored`" 로 총함수를
  만들면 짧지만 **`Ready` 가 남아 있는 버그를 조용히 덮는다**("bout 종료 시 커서는 항상 끝"이 깨진 경우)

**주사위 에셋** — 카운터·회피가 **현재 카드 3장에 둘 다 없다**(`Guard` Block / `Combo` Attack /
`Execute` Attack·Block·Attack). 브레이크포인트를 걸려면 새로 만들어야 한다.
`Evade` 를 크게(예: 6~6) 잡아 `Combo`(Attack 1~3)에 이기게 하는 게 제일 쉽다.
`DiceData.Type` 은 enum 이라 기본값이 `Attack`(0)이니 드롭다운을 반드시 확인할 것 —
`Execute` 에서 이미 겪었고 증상이 "분기가 안 돈다"와 똑같이 보인다.

#### 검증 결과 (2026-08-05, 플레이 검증 완료)

**계기는 출혈이다 — 출혈 스택이 곧 굴림 카운터다.** 3-1.5 이후 출혈은 주사위를 굴릴 때마다
터지고 스택이 절반으로 준다(5→2→1). 3-1.9 덕분에 일방에서는 Attack 만 굴러가므로 방어
주사위가 카운트를 오염시키지 않는다. `DebugAddBleed` 버튼과 `StatusUI`(characterId 0 = Ally)가
이미 씬에 있어 **추가 배선도 브레이크포인트도 0**이다. 앞으로 "몇 번 굴렀나"를 물어야 하면 이걸 쓸 것.

**A. 주사위 순서 — 통과.** `Execute` 일방 공격에서 Ally 출혈 5→2(굴림 1회) =
첫 주사위 50 이 즉사시켜 뒤 둘이 잘렸다. 역순이면 3~6 이 먼저 맞고 살아남아 2회가 된다.
- **초판의 판정 기준("Enemy HP 감소량")은 틀렸다.** 데미지 총합은 순서와 무관하고(덧셈)
  50 이 어디 있든 HP 50 인 Enemy 는 죽으므로 **양쪽 가설이 똑같이 0 을 예측한다.** 무정보다.
  실제로 "딱 0" 을 받고 통과로 처리할 뻔했다

**B. 저장분이 다시 나오는가 — 통과.** bout1 에서 Ally `Guard` 일방(Block 저장) →
같은 턴 bout2 에서 Ally `Strike`(1개) ↔ Enemy `Combo`(3개) 합. Ally 출혈 5→2→1(굴림 2회) =
Strike 가 떨어진 뒤 `Peek` 이 저장된 Block 으로 이어졌다.
- **같은 턴이어야 한다.** `ResetForNextTurn` 이 `_dice.Clear()` 라 저장분은 턴을 못 넘긴다
- ~~4단계 전이라 "제3자에게 일방으로 맞을 때"는 아직 안 된다~~ — **4단계와 함께 확인됨
  (2026-08-05 후속).** 액션 없는 슬롯을 지목한 `Strike` 일방 피격에서 Ally 출혈 5→2
- **덤으로 `ResolveCombat` 둘째 while 루프 정상 경로도 확인됐다**(2026-07-30 미확인 항목).
  적 Combo 3개 중 남은 것이 일방으로 흘러간다
- **위양성 경로 하나**: Guard 슬롯이 합이면(`Bout: -` 가 아니면) Block 이 굴러서 똑같이 2회가 나온다.
  턴 종료 전 슬롯 표시로 확인할 것

**검증 세팅 요령** — 동점을 이용해 bout 순서를 결정론으로 만든다. `Ally01` 속도를 **5/5 고정**,
`Enemy01` 을 **1/1** 로 두면 아군 두 슬롯이 항상 동점이라 **슬롯 0 이 확정으로 먼저** 돌고
적 슬롯은 항상 마지막이다. SPD 를 읽고 재시작할 필요가 없어진다.
덱은 드로우가 턴당 1장이라 **턴N 손패 = N장 확정**. `Ally01` 현재 덱은 4장
(Strike / Guard / Evade / Counter — 2026-08-06 에 5단계 검증용으로 교체).

**C. 회귀** — 손패 총 장수 보존(8-B 의 지표), 1대1 기존 전투가 그대로 도는지.
**4단계 뒤에 실시, 통과** (2026-08-05 후속). 저장분 합 이어짐(5→2→1)까지 3건.

**함정 재발 주의**: 브레이크포인트가 거짓 신호를 준 적이 있다(Code Optimization 이 Release).
음성 결과는 **양성을 한 번 본 뒤에만** 의미가 있다.

#### 미확인 (사용자 확인 필요)

- ~~**행동 순서 동점 규칙**~~ — ✔ **2026-08-20 사용자 확정: 속도 동점이면 `CharacterId` 오름차순.**
  **코드 수정 0건 — 추측으로 둔 것이 맞았다.** `ActionPriority.CompareTo` 가 max-heap 기준
  (먼저 나올 것이 커야 한다)이고 `other.CharacterId.CompareTo(CharacterId)` 라 작은 id 가 커진다.
  슬롯도 같은 모양이라 슬롯 0 이 먼저다
  - **규칙은 "아군 먼저" 가 아니라 "id 가 작은 쪽 먼저" 다.** 지금 아군이 먼저 나오는 것은
    `BattleSnapShot` 이 아군→적군 순으로 id 를 매기기 때문에 생기는 **결과**일 뿐이다.
    id 부여 순서가 바뀌면(관전·자동 전투 등) 행동 순서도 따라 바뀐다 — 팀을 보는 코드는 없다
- ~~**저장분은 턴을 넘기나?**~~ — **2026-08-05 확정: 안 넘긴다.** 턴이 끝나면 초기화된다.
  `ResetForNextTurn` 은 지금 그대로(`_dice.Clear()` + `_cursor = 0`)고,
  이 메서드의 존재 이유가 **"저장분 버리기" 하나**로 확정됐다
- **`BoutStartLog` / `BoutEndLog` 에 합 여부를 실을 것** (사용자 결정: "로그는 최대한 정확한
  정보를 남기는 게 맞다"). 지금 `BoutEndEvent` 는 `DefenderId` 를 받아놓고 **읽지 않는다**(쓰기 전용).
  로그가 이벤트보다 정보가 적은 상태다. 합이었는지는 **다른 로그로 역산이 안 된다** —
  한 bout 안에 합과 일방이 둘 다 들어가므로 로그 종류로는 구분이 안 된다.
  **타입은 `bool WasClash`** (2026-08-06 결정, 8번 항목에 근거)

#### 참고 — 확정된 규칙과 자료구조

**원작 규칙 (사용자 확인 완료)**
- 주사위 큐 = **`[이번 카드 주사위]` + `[저장분]`** — 이 순서다. 저장분이 뒤다
- 합에서 **블락 / 회피 / 카운터** 주사위가 맞붙을 상대가 없으면 굴리지 않고 **저장**된다
- 저장분은 큐가 이어지는 것뿐이라 **두 경우 모두**에서 나온다:
  주사위가 상대보다 모자라 합이 이어질 때 / **일방 공격을 당할 때(합 상대가 아닌 제3자 포함)**
- `Used`(반격 재사용) 주사위는 **턴이 끝날 때** 소멸한다.
  ~~(지금은 합 끝)~~ — **이 괄호는 틀렸다. 5단계 항목 참고: `DestroyUsed()` 는 도달 불가 코드라
  bout 끝에 죽인 적이 없다.** 실제로는 `Consumed` → `Stored` 로 턴 내내 살아남고 있어
  결과적으로 원작 규칙과 일치한다
- **저장분은 턴을 넘기지 않는다** (2026-08-05 확인). 턴 끝에 초기화

**확정된 자료구조** — `[새, 새, 소비, 소비]` 로 두고 커서는 항상 앞에서 시작한다:
```
bout 끝:   Consumed → Stored / Destroyed 를 _dice 에서 제거 / _cursor = 0   (A, B, Target 전부)
bout 시작: _dice.InsertRange(0, 새 주사위)
Peek:      커서부터 전진, Ready | Used | Stored 를 반환
```
이 순서를 고른 이유와 접은 대안은 3-1.10 항목에 있다. **커서가 절대 뒤로 안 가는 것이 핵심**이고,
죽음(`DestroyRemaining`)·중단(`DiscardRemaining`) 처리가 저장분까지 공짜로 덮는 것이 결정타였다.

**설계 근거 전체**(접은 대안과 그 이유 포함)는 아래 "3. 상태이상 — 남은 것"의
3-1.10 / 3-1.12 항목에 있다. 구조를 다시 흔들고 싶어지면 먼저 읽을 것.

**검증 세팅**: 저장분은 "한 bout 에서 채워지고 다른 bout 에서 쓰이는" 물건이라
**`CharacterData.SpeedSlotCount` 를 2로** 올려야 관측된다. Ally 가 `Guard` 로 일방(저장분 채움) →
Enemy 가 `Strike` 로 Ally 일방 공격(저장분 소모) 순서.

**곁들일 만한 작은 것들 (독립적, 몸풀기용)**
- **3-1.11** — `DiceClashLog` 가 보정 전 `CurrentRoll` 을 찍는다. 3-1.7 검증이 끝났으니 이제 고쳐도
  안전하다(그때는 "훅이 도는 것"과 "로그가 바뀐 것"이 섞여서 일부러 미뤘다)
- ~~**`ResetForNextTurn` 이 `_dice.Clear()` 를 해야 하나**~~ — **2026-08-04 결정됨. 3-1.12 로 승격.**
  턴 끝이 아니라 **bout 끝마다** 파괴분을 제거하기로 했다. 3-1.10 과 한 덩어리로 간다
- **`DiceDiscardRemainingLog` / `DiceDestroyRemainingLog`** — 이번엔 디버거로 검증했지만
  다음부터 브레이크포인트 없이 보려면 필요하다. 개수를 실으려면 `DicePool.DiscardRemaining` 을
  `void` → 반환으로(`AddStatus` 가 결과 스택을, `EnergyRecoverEvent` 가 실제 회복량을 싣는 패턴)

**대안**: "4. Engine → Data 의존 끊기 2단계"(`CharacterModel`)가 주사위·상태이상과 완전히
독립적이라 머리를 쉬어가는 선택지.

**검증 세팅 참고 (이번 세션에 확립됨)**
- 적 AI 가 없고 `ActionRegisteredEvent` 에 소유자 검사도 손에서 카드를 빼는 로직도 없다.
  아군 손의 카드를 **적 슬롯에도 걸 수 있고** 같은 턴에 양쪽으로 쓸 수도 있다
- 덱은 셔플 + 턴당 1장 드로우라, 한 항목에 집중할 때는 **그 카드만 3장** 넣는 게 빠르다
- 카드: `Strike`(Attack) / `Guard`(Block 4~7) / `Combo`(Attack 1~3 ×3) /
  `Execute`(Attack 50~50, Block 4~7, Attack 3~6) / **`Evade`(Evade 6~6)** / **`Counter`(Counter 6~6)**.
  `Execute` 는 검증 전용이다 — 즉사 주사위로 "대상 사망 후 잔여 처리"를 만든다.
  `Evade`/`Counter` 는 6~6 고정이라 `Combo`(1~3)에 **확정으로 이겨** `Reuse` 를 만든다
- **새로 만든 에셋은 그 자체가 검증 대상이다.** `DiceData.Type` 은 enum 이라 기본값이 `Attack`(0)이고,
  인스펙터에서 Min/Max 만 채우고 드롭다운을 안 건드리면 조용히 공격 주사위가 된다.
  실제로 `Execute` 에서 겪었고, 증상이 "분기가 안 돈다"와 완전히 똑같이 보였다
- **에셋을 고쳤으면 Unity 에서 `File > Save Project` 를 눌러야 디스크에 쓰인다** (2026-08-06).
  ScriptableObject 편집은 메모리에서 dirty 로만 있고, 에디터 안에서는 정상으로 보이며 플레이도
  정상으로 돌아간다. **커밋 직전에 `git status` 로 에셋이 비어 있지 않은지 확인할 것** —
  실제로 `Evade`/`Counter` 가 `_dices: []` 인 채로 커밋될 뻔했다(검증은 통과한 상태에서)

### 완료 (이번 세션)
- **4번 4단계 — Engine asmdef + No Engine References (2026-08-21, 플레이 검증)**.
  DEVLOG `2026-08-21` 참고. 설계 근거는 4번 항목의 "4단계" 절.
  **이것으로 4번(Engine → Data 의존 끊기)이 통째로 끝났다.**
  - `LOR.Engine.asmdef` 신설. `noEngineReferences: true` / `autoReferenced: true` / `references: []`
  - **게이트가 둘이다** — `Engine` → `UnityEngine` 은 체크박스가, `Engine` → `Data`/`Scene`/`UI` 는
    **어셈블리 분리 그 자체**가 막는다(반대 방향이 순환 참조가 되어 Unity 가 거부).
    둘째가 이 항목의 원래 제목이고, 공짜로 딸려왔다
  - **검증을 `rsp` 파일로 했다** — `LOR.Engine` 의 `UnityEngine` 참조 0건 /
    `Assembly-CSharp` 는 134건. **같은 grep 이 대조군을 겸해서** "참조 없음" 과
    "grep 이 안 걸림" 이 갈렸다. 위양성 경로가 없는 관측을 **고르는 단계에서** 확보한 첫 사례
  - `internal` 이 Engine 에 0건이라 `CS0122` 가 안 났고, `namespace` 가 0개(전부 global)인 것도
    문제가 안 됐다(어셈블리 경계와 네임스페이스는 별개)
  - **덤으로 발견**: `Engine/Events/DamageEvent.cs:2` 의 `using System.Runtime.InteropServices;` 가
    **안 쓰인다**(파일 38줄에 `StructLayout`/`Marshal`/`DllImport` 0건). 주석 작업 때 같이 지울 것
- **`DamageLog` / `StaggerLog` 에 공격자 (2026-08-06, 플레이 검증)**.
  DEVLOG `2026-08-06 (후속 4)` 참고.
  - `DamageLog` → `{AttackerId, TargetId, Amount}`. 호출부 1개(`DamageEvent.cs:22`)
  - `StaggerLog` → `{AttackerId, CharacterId, Amount, IsRecover}`.
    **`CharacterId` 를 `TargetId` 로 개명하지 않았다** — `DamageLog` 과 모양을 맞추고 싶어지지만
    회복 케이스에서 자기 자신이 "Target" 이 되는 게 거짓말이다. 덕분에 `StaggerUI.cs:52` 무수정
  - **`StaggerLog.CharacterId` 는 "한 필드가 두 뜻" 처럼 보이지만 아니다.**
    `IsHeal ? Attacker : Defender` 로 계산되지만 뜻은 "누구의 스태거가 변했나" 하나로 일관된다.
    **계산식이 갈리는 것과 의미가 갈리는 것은 다르다** — 오늘 같은 병을 두 번 고친 뒤라
    반사적으로 같은 것으로 보였다. 패턴 인식이 붙으면 위양성도 같이 는다
  - **회복일 때 `AttackerId == CharacterId` 인 것은 `DefenderId` 문제가 아니다.**
    `DefenderId` 는 **어떤 경우에도** 다를 수 없어 0비트였고, 이건 막기 승리에서 실제로 다르다.
    판단 기준은 **"다를 수 있는 경우가 하나라도 있는가"**
  - `DiceUI` 에 `StaggerLog` 구독 추가 — **없으면 고쳐도 확인이 안 됐다**
    (`[Clash] ... => StaggerContext` 까지만 보이고 결과가 안 보였다)
- **`BoutStartLog`/`BoutEndLog` 에 합 여부 + `DefenderId` → `bool WasClash` (2026-08-06, 플레이 검증)**.
  DEVLOG `2026-08-06 (후속 3)` 참고. 설계 근거는 8번 항목.
  - **`DefenderId` 는 4단계에서 지운 `idB` 의 살아남은 쌍둥이였다** — 같은 `BoutStartEvent` 의
    같은 식(`B?.SourceSlot.CharacterId`)이고, 4단계가 `ResolveCombat` 쪽만 정리했다.
    나르는 정보는 **정확히 1비트**이고 `int` 부분은 `TargetId` 의 복사본이라 0비트다
  - `BoutStartEvent.Apply` 가 `bool wasClash = B != null;` 을 한 번 계산해 `AddLog` 와
    `BoutEndEvent` 두 곳에 쓴다. `BoutEndEvent` 의 쓰기 전용 필드가 해소됐다
  - `DiceUI` 에 `BoutStartLog` 구독 추가 → **로그 콘솔에 bout 구분선**이 생겼다.
    `BoutEndLog` 은 일부러 구독 안 했다(줄이 두 배가 되는데 얻는 게 없다. 필요하면 그때)
  - **`AddLog` 가 `UseAction` 앞인 것은 그대로 뒀다.** "상태 먼저 → 로그" 규칙의 취지는
    "UI 가 모델을 다시 읽을 때 헌 값을 보면 안 된다"인데 이 로그는 모델을 안 읽는다.
    bout 구분선은 오히려 주사위 적재 로그보다 먼저 찍혀야 맞다
  - **검증 출력에서 "짝인데 한쪽만"을 두 번 했다** — 일방 분기에 라벨이 없어
    "`WasClash` 가 항상 false" 여도 정상으로 보이던 것, 그리고 고친 뒤 한쪽만 앞 공백이 빠진 것.
    **삼항 연산자는 이 실수가 잘 나는 자리다**
- **`IsOffensive` 확장 메서드 — 3-1.13 종결 (2026-08-06, 검증 완료)**.
  DEVLOG `2026-08-06 (후속 2)` 참고. 동작 변화 0인 순수 리팩터링.
  - `DiceTypeExtensions.IsOffensive(this DiceType) => Attack || Counter`, `DiceData.cs` 의 `DiceType` 옆.
    **C# enum 은 멤버를 선언할 수 없으므로 확장 메서드가 유일한 관용구다** — 취향이 아니라 언어 제약
  - 호출부는 `StatusEffects.cs:41`(힘) / `AttackBoostPassive.cs:14` 두 곳뿐.
    `CombatExecutor.cs:162` / `DicePool.cs:81` 은 **안 건드렸다**(저장 규칙이지 공격형 판정이 아니다)
  - **긍정형 이름이 실익의 절반이다.** `!= A && != B` 는 드모르간을 머리로 돌려야 읽히고
    `&&` 를 `||` 로 잘못 쓰면 조건이 항상 false 가 되어 **가드가 통째로 사라진다**(전 타입에 힘이 붙음)
  - **첫 제출에서 두 곳 중 한 곳만 바뀌어 있었다.** 안 고친 것보다 나쁜 상태다 —
    같은 뜻인 두 줄이 서로 다른 모양이 된다. 어제 배운 점에 "짝인데 한쪽만 고치는 실수"를
    적어놓고 바로 다음 작업에서 반복했다
- **3-1.11 + `UnopposedLog` 배선 + 전역 로그 콘솔 (2026-08-06, 플레이 검증)** —
  로그가 사실을 못 담고 있던 것 셋을 한 덩어리로. 자세한 것은 3-1.11 항목 참고.
  - `DiceClashLog` 에 `BaseRollA/B` + `ModifiedRollA/B` 두 쌍
  - `UnopposedLog` 배선 (생산자 0건이었다) — **일방 공격은 로그를 아예 안 남기고 있었다**
  - **`DiceUI`** 신설 (`UI/Status/`) — `LogDispatcher` 6종 구독하는 **전역** 콘솔.
    `StatusUI` 와 구조가 다르다: 거기선 로그가 트리거고 데이터는 모델에서 오는데,
    **여기선 로그가 곧 데이터**다(굴림값은 모델에 안 남는다). 그래서 `Refresh()` 가 아니라
    `Append(line)` + `Redraw()`. 라벨은 **ASCII** 다 — TMP 기본 폰트에 한글 글리프가 없다
  - `CharacterRuntime.GetDiceInfo(int diceId)` + `DiceInfo` readonly struct 신설.
    **`DiceEntry` 를 밖으로 안 내보내는 게 요점** — 내보내면 UI 가 `CurrentRoll`(덮어써진
    마지막 값)을 읽어서 로그 옆에 나란히 틀린 숫자를 찍는다. 3-1.12 의 "불변 속성만" 규칙을
    주석이 아니라 **타입이 강제**하게 만든 것
- **3-1.13 — 힘/`AttackBoostPassive` 가 `Counter` 를 안 올려주던 것 수정 (2026-08-06, 플레이 검증)**.
  `DiceUI` 에서 `[6->9]` 화살표로 확인. `IsOffensive` 헬퍼는 남았다(위 후보 1번)
- **3-1.10 5단계 완료 (2026-08-06, 브레이크포인트 검증)** — **`DiceState.Used` 가 한 번도
  안 찍히고 있었다.** DEVLOG `2026-08-06` 참고. 설계 근거는 위 "5단계" 절.
  - `ToAdvanceEvent` 의 `AdvanceType.Reuse => null` → `new DiceReusedEvent(characterId)`.
    `DiceReusedEvent` / `DiceReusedLog` 신설 (이름은 `Consumed`/`Destroyed` 와 맞춰 과거형)
  - `DestroyUsed()` **삭제**, `StoreConsumed` → **`StoreSurvivors`** (`Consumed` 와 `Used` 를
    둘 다 `Stored` 로). 옛 `DestroyUsed` 는 죽은 코드일 뿐 아니라 **규칙상 틀렸다** —
    살아났으면 반격 주사위를 bout 끝에 죽여서 "턴 끝까지 산다"를 어긴다
  - **`Ready` 는 일부러 안 건드린다.** "`Destroyed` 아니면 전부 `Stored`" 로 총함수를 만들면
    짧지만 **"bout 종료 시 커서는 항상 끝" 위반을 조용히 덮는다.** 명시적으로 나열해야
    남은 `Ready` 가 다음 bout 의 `Peek` 에 걸려 증상으로 드러난다
  - **관측 가능한 동작 변화가 0이었다** — 배선 전에도 `Reuse` 가 원하는 것("커서를 안 움직인다")이
    아무것도 안 하면 달성됐다. 그래서 검증은 브레이크포인트 하나뿐이고, 검증용
    **Evade 카드 에셋을 새로 만들었다**(기존 3장에 Counter/Evade 가 없었다)
  - **`EndBout` 의 `Used → Stored` 는 지금 도달 불가다**(모든 탈출 경로가 커서의 주사위를
    다시 집어 `Consume`/`Destroy` 한다). 검증 대상이 아니라 보험
- **3-1.10 4단계 완료 (2026-08-05, 플레이 검증)** — 일방으로 맞는 쪽의 저장분이 이제 방어에 나온다.
  DEVLOG `2026-08-05 (후속)` 참고. 설계 근거는 위 "4단계" 절.
  - `ResolveCombat(ActionInstance a, ActionInstance b, int idA, int targetId)`
    → **`ResolveCombat(int attackerId, int targetId)`**. `a`/`b`/`idB` 전부 소멸.
    `a` 는 원래부터 본문에서 한 번도 안 쓰였다
  - 첫 루프 `diceB = Peek(targetId)`, 둘째 루프의 `if (b != null)` 감싸개 삭제.
    **`ResolveUnopposedDice` 는 무수정** — 도달했다는 것 자체가 "대상 풀이 비었다"는 뜻이 되어
    남아 있던 `if (Type != Attack)` 이 곧 "맞붙을 상대가 없으니 저장"이 된다
  - **새로 짠 해석 로직 0.** `ResolveDiceClash` 가 "B 가 합 상대인가"를 묻지 않으므로
    (3-1.7 이 `isOwnerA` 를 없앤 결과) 그대로 재사용됐다
  - `diceB` 는 두 가드 **뒤**에서 뽑는다 — `Peek()` 은 시체를 만나면 `_cursor` 를 미는
    부작용이 있어 순수 읽기가 아니다. 앞에 두면 죽은 대상의 풀을 매 루프 건드린다
  - **`ResolveUnopposedDice` 의 `if (attacker.IsDead) return;`(`CombatExecutor.cs:179`)은
    주사위를 전진시키지 않고 빠지는 유일한 경로다.** 무한 루프가 안 나는 근거가 이 파일이 아니라
    `DeathEvent.cs:14` 의 `DestroyRemainingDice()` 에 있다("`ResolveCombat` 의 while 에서
    아무것도 안 하고 빠지기는 없다"는 규칙의 예외). 4단계로 **일방 경로에서도 도달**하게 됐다
  - **일방 피격자에게 저장분이 남으면 둘째 루프가 그걸 `Consume` → `EndBout` 이 다시 `Store`.**
    상태는 제자리고 `DiceConsumedLog` 만 하나 더 나간다. 버그 아님
  - **설계 메모 두 항목이 틀렸었다**(둘 다 "실제 규칙 차이다"라고 단정해뒀던 것). 정정 내용과
    이유는 "4단계" 절에 취소선으로 남겨뒀다 — 지우면 같은 결론을 다시 내게 된다
- **행동 순서가 뒤집혀 있던 것 수정 (2026-08-05, 플레이 검증)** — **느린 슬롯이 먼저 행동하고 있었다.**
  DEVLOG `2026-08-05` 참고.
  - `PriorityQueue` 는 **max-heap** 이다(`PriorityQueue.cs:39` 자식>부모일 때 올림, `56~57` 큰 자식 선택.
    변수명이 `smallest` 라 반대로 읽힌다). 그런데 `ActionPriority.CompareTo` 의 속도 줄이
    `other.Speed.CompareTo(Speed)` 로 뒤집혀 있어 **두 반전이 겹쳤다.**
    동점 처리도 같은 이유로 반대였다(적이 아군보다 먼저, 슬롯 1 이 슬롯 0 보다 먼저)
  - 수정은 세 줄의 인자 방향을 max-heap 기준으로 정렬. 속도는 정상(`Speed.CompareTo(other.Speed)`),
    `CharacterId`/`SlotIndex` 는 반대로. **"먼저 나와야 할 것이 `CompareTo` 상 커야 한다"**
  - **`PriorityQueue` 를 min-heap 으로 바꾸는 선택지는 남아 있다.** 그러면 `CompareTo` 가
    "작을수록 먼저"라는 표준 관례가 되고 .NET 표준 타입으로 갈아탈 때도 안 깨진다.
    지금은 영향 범위 때문에 `ActionPriority` 만 고쳤다
  - **왜 지금까지 안 보였나**: 기존 속도가 Ally 1~6 / Enemy 7~9 라 뒤집힌 순서에서 아군이 먼저
    나왔고, 그게 기대와 우연히 일치했다. 규칙과 정반대인데도 화면상 순서가 같아 모순이 안 보였다.
    드러난 계기는 코드 리뷰가 아니라 **검증하려고 속도를 극단으로 바꾼 것**이다
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
  - ~~`Refresh()` + `readonly _baseDuration`. 단 현재 규칙에선 영구 no-op … 데드 코드 정리 후보~~ —
    **2026-08-21 실측 정정. 이 줄은 틀렸다.**
    - **`Refresh()` 는 존재하지 않는다.** `StatusEffectRuntime` 에 그런 메서드가 없다
      (만들지 않았거나 이후 지워졌고, 어느 쪽이든 문서만 남았다). **정리할 데드 코드가 없다**
    - **`_baseDuration` 은 살아 있고 no-op 도 아니다.** `TickTurnEnd`(`StatusEffectRuntime.cs:53`)의
      `Duration = _baseDuration` 재장전이 유일한 독자이고, **3-2 Delay 의 대기분 승격이 이 줄에 의존한다**.
      지웠으면 그게 깨졌다
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
  - **범위 규칙**: 힘 = **공격형**(`Attack` + `Counter`), 마비 = 전 타입. 효과마다 다르므로 타입 검사는 훅 안.
    ~~힘 = 공격 주사위만~~ — **2026-08-06 수정.** 아래 "3-1.13" 참고
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
  - **첫 루프 `DiceDiscardRemainingEvent` + 보관 분기 검증 완료 (2026-07-30 후속 세션).**
    증거는 `DicePool.cs:88`(`dice.Consume();`) 브레이크포인트가 걸린 것 — 그 줄이 보관 분기 자체다.
    - **위양성을 한 번 겪었다.** `Recover()` 에서 본 `[Destroyed, Consumed, Destroyed]` 로 통과 판정했는데,
      그 배열은 **역순 적재 시에도 똑같이 나온다**(`Consumed` 가 `ResolveUnopposedDice` 의 비-Attack
      분기에서 온 것). **관측 지점은 "값이 보이는가"가 아니라 "두 가설을 갈라주는가"로 골라야 한다.**
  - **둘째 루프 정상 경로는 아직 미확인** — Ally `Strike`(1개) ↔ Enemy `Combo`(3개) 합으로 만들 수 있다.
- **카드 3장 추가 + 3-1.5 / 3-1.7 검증 완료 (2026-07-30, 플레이 검증)** —
  DEVLOG `2026-07-30 — 카드 3장 추가...` 및 그 후속 섹션 참고.
  - `Guard`(Block 4~7) / `Combo`(Attack 1~3 ×3) / `Execute`(Attack 50~50, Block 4~7, Attack 3~6).
    엔진 변경 0, Data 계층만. 덱은 `Ally01` 만 고치면 된다(적 AI 가 없어 아군 카드를 적 슬롯에 걸 수 있다).
  - **3-1.5 는 반드시 합이어야 한다** — 3-1.9 가 일방에서 방어·회피를 안 굴리게 바꿨으므로 방어 주사위가
    굴러가는 자리는 `ResolveDiceClash` 뿐이다. 그래서 3-1.5 와 3-1.7 은 같은 카드 / 다른 세팅이고
    서로의 대조군이 된다.
  - **오진 3연속을 겪었다.** 브레이크포인트가 두 번 거짓 신호(Code Optimization 이 Release 였다),
    한 번은 에셋 데이터 오류(`Execute` 의 Block 이 `Type: 0` = Attack). 관측이 게임 상태와 모순되면
    **코드보다 계기를 먼저 의심할 것.** 음성 결과는 양성을 한 번 본 뒤에만 의미가 있다.
  - `Step()` 은 이벤트를 유실하지 않는다 — `EnqueueEvent` 가 1개 넣고 `Step()` 이 1개 빼는 쌍이 원자적이라
    큐는 호출 밖에서 항상 비어 있다. 위험한 건 유실이 아니라 **예외 전파**(DFS 스택이 깊어
    UI 콜백 하나가 던지면 `ResolveCombat` 의 while 까지 풀린다).
- **`ResetDiceForNextTurn()` 배선 복구 — 유실된 호출 (2026-07-30, 플레이 검증)** —
  호출자가 0건이었다. `TurnEndEvent.Apply` 가 부르기로 되어 있었으나(DEVLOG:2746) 유실돼 있었다.
  - 자리는 `TriggerTurnEnd()` **뒤**, `AddLog` **앞**. 앞이면 턴 종료 훅이 비워진 풀을 보고,
    뒤면 `AddLog` 가 동기라 UI 가 변경 전 값을 읽는다(2026-07-22 `DamageEvent` 와 같은 구도).
  - 이벤트를 새로 만들지 않고 `Event.Apply` 안에서 직접 호출 — `DeathEvent` 의 `DestroyRemainingDice` 선례.
  - **`ResetForNextTurn` 의 두 줄은 짝이다.** `_cursor = 0` 은 나머지가 전부 `Destroyed` 라는 보장 위에서만
    안전하다. 커서만 되돌리면 다음 턴 `Peek` 이 지난 턴의 `Ready` 주사위부터 집는다.
  - 검증은 `DicePool.cs:64` 의 **대입 전 `_cursor`** 하나로 했다. `Combo` ×3 덱 2턴 → 1턴 `3` / 2턴 `6`.
    6 이 `Peek` 이 시체 구간을 건너뛰었다는 증거다. 적 풀은 `_dice.Count == 0` 이라 구분 기준이 됐다.
  - **`_dice` 누적은 남아 있다**(2턴차 `Count` 6). `Clear()` 여부는 별건으로 미뤘다 — 한 번에 둘을 바꾸면
    회귀 원인을 구분할 수 없다. 리플레이에는 영향 없음(아래 "주사위 잔존과 리플레이" 참고).
- **주사위 역순 적재 수정 (2026-07-30 후속 세션, 플레이 검증)** —
  `DicePool.Add` 의 호출자가 0건이고 적재가 `Inject`(= `_dice.Insert(_cursor, entry)`) 로 되어 있었다.
  턴 시작에 `_cursor` 가 0이라 `Insert(0, ...)` 을 반복하면 **카드에 적은 주사위 순서가 뒤집힌다.**
  `UseAction` 의 적재 루프를 `Add` 로 교체.
  - **`Add` 가 순서를 보존하는 쪽이다.** `Peek()` 이 `_cursor` 부터 인덱스 증가 방향으로 훑으므로
    "뒤에 넣고 앞에서 읽기" = FIFO 다. 뒤집히는 조합은 "앞에 넣고 앞에서 읽기"였다.
    스택(`Push`/`Pop`) 직관과 반대라 헷갈리기 쉽다.
  - `Inject` 는 남긴다 — 용도가 다르다(전투 중 큐의 현재 위치에 끼워 넣어 다음에 바로 쓰기).
    단 **여러 개를 연속 `Inject` 하면 같은 역순 함정**이다.
  - **커서가 리스트 끝이면 `Insert(count, e) == append` 라 두 메서드가 일치한다.** 그래서 "전투 중 추가"는
    정상으로 보이고 초기 적재만 깨져 있었다. 게다가 카드가 전부 주사위 1개(`Strike`/`Guard`)거나
    동일 주사위(`Combo`)여서 순서가 관측 불가능했다 — 비대칭 카드 `Execute` 가 생기고서야 드러났다.
  - `Add` × `ResetDiceForNextTurn` 조합 회귀 확인: `Combo` 2턴에서 `_cursor` 1턴 `3` / 2턴 `6`.
    `Add` 가 새 주사위를 시체 뒤에 붙이는데 `Peek` 이 커서 0에서 시체를 건너뛰어 도달한다 —
    리셋이 잔여를 전부 `Destroyed` 로 만들어주기 때문에 성립한다.
- **주사위 잔존과 리플레이 (2026-07-30 후속 세션, 설계 토론)** —
  결론: `DicePool._dice` 에 파괴된 주사위를 남겨도 **리플레이에 기여하지 않는다.**
  - 리플레이 = **`BattleSnapShot`(Seed + `CharacterState`) + 플레이어 입력 순서열**. `IRng` 가 시드 기반이고
    나머지가 순수 계산이라 이 둘로 재생된다. 파괴된 주사위는 어떤 판단의 입력도 아니다(`Peek` 이 건너뛴다).
  - **가변 객체는 이력이 아니다.** `DiceRuntime.CurrentRoll` 은 `Roll()` 마다, `State` 는 전이마다 덮어써진다.
    보관해도 **마지막 상태만** 남는다. `Reuse`(반격 재굴림)는 지금도 앞선 굴림값을 잃는다.
  - 이력이 사는 곳은 `BattleRuntime.CombatLogs`(`IReadOnlyList<CombatLog>`). `DiceClashLog` 가
    `DiceHandle` + **그 순간의** 굴림값을 싣는다. `_nextDiceId` 는 캐릭터당 한 번 초기화되고 리셋되지
    않으므로 `DiceHandle` 이 전투 내내 유일하다 — id 설계는 이미 리플레이/이력을 제대로 받친다.
  - **`_diceById` 가 쓰기 전용이다** (`CharacterRuntime.cs:227` 에서 쓰기만, 읽는 곳 0건).
    DEVLOG:2022 에 "이벤트에서 id 기반 주사위 추적용"으로 적혀 있으나 배선이 안 왔다.
    `Clear()` 를 정하기 전에 **이것의 용도를 먼저 결정할 것** — 기록 담당이면 `Clear()` 는 공짜,
    죽은 코드면 지우고 기록 책임을 로그에 둔다. 같은 것을 두 곳에 쌓으며 아무도 안 읽는 현 상태가 문제.
  - ~~**미확인 전제**: "리플레이"가 (A) 처음부터 재생인지 (B) 중간 상태 저장/복원인지~~ —
    ✔ **2026-08-20 사용자 확정: (A) 처음부터 재생.** 따라서 위 결론이 그대로 성립한다 —
    리플레이는 **시드 + 입력열**이고, 중간 상태 직렬화 논의는 필요 없다.
    `BattleSnapShot`(Seed + `CharacterState`)이 이미 그 형태고, 이제 `CharacterState` 가
    SO 를 안 물고 있으므로(4번 2단계) 스냅샷이 진짜로 순수 데이터가 됐다

### 3. 상태이상 — 남은 것

- ~~**3-1.10. 주사위 저장분**~~ — ✔ **종료 (2026-07-30 발견 → 08-04 설계 확정 →
  08-05-06 1~5단계 검증 완료)**. 아래는 설계 근거 기록이다.
  **주사위 큐 구조를 다시 흔들고 싶어지면 먼저 읽을 것**

  **증상.** `DiceRecoverEvent` 가 실효가 없다. `Advance(Consume)` 은 `_cursor++` 를 하는데
  `Recover()` 는 **상태만** 되돌리고 커서는 안 건드린다. `Peek()` 은 `_cursor` 부터 앞으로만
  훑으므로 되살아난 주사위는 **커서 뒤에 갇힌다.** 로그만 나가고 게임에는 아무 영향이 없다.
  - ~~`ResetForNextTurn` 이 `_cursor = 0` 과 함께 전부 `Destroy` 한다~~ — **초판의 이 전제는 틀렸다.**
    그때 `ResetForNextTurn` 은 호출자가 0건이라 아예 안 돌고 있었다. 2026-07-30 배선 복구로 참이 됐다.

  **`Consumed` 의 생산자는 전투 전체에서 딱 둘이다** — `CombatExecutor.cs:167`(`ResolveUnopposedDice`
  의 비-Attack 분기)과 `DicePool.cs:88`(`DiscardRemaining` 의 비-Attack 분기). `AdvanceType.Consume` 은
  **`DiceRuleTable` 에 한 번도 안 나온다.** 즉 `Consumed` = "일방이라 안 굴린 방어 주사위" 라는 뜻이
  코드에 이미 박혀 있다.

  **원작 규칙 (2026-08-04 사용자 확인 완료)**
  - 큐 = **`[이번 카드 주사위]` + `[저장분]`**. 저장분이 **뒤**다
  - 블락 / 회피 / 카운터가 맞붙을 상대가 없으면 굴리지 않고 저장
  - 저장분은 큐가 이어지는 것뿐이라 **주사위가 모자라 합이 이어질 때**도,
    **일방 공격을 당할 때(합 상대가 아닌 제3자에게 맞아도)**도 나온다
  - `Used`(반격 재사용)는 **턴 끝**에 소멸 (지금은 합 끝)

  **확정 설계 — `[새, 새, 소비, 소비]` + 항상 앞에서 시작하는 커서**
  ```
  bout 끝:   Consumed → Stored / Destroyed 를 _dice 에서 제거 / _cursor = 0
  bout 시작: _dice.InsertRange(0, 새 주사위)
  Peek:      커서부터 전진, Ready | Used | Stored 를 반환
  ```
  - **커서가 절대 뒤로 안 간다**는 게 핵심이다. 그래서 "같은 bout 에 저장한 걸 또 집는"
    무한 루프가 구조적으로 불가능하고, `Peek` 이 단순 전진 루프 한 개로 끝난다
  - **죽음·중단이 공짜로 맞는다.** 저장분이 커서 **앞**에 있으므로 `DestroyRemaining`(죽음)이
    같이 지우고 `DiscardRemaining`(중단)은 `Consume` 재적용이라 그대로 유지된다.
    아래 접은 대안에서는 이게 **버그**였다
  - **리스트 순서 = 논리적 큐 순서.** 디버거로 `_dice` 를 열면 보이는 게 곧 사실이다

  **접은 대안 (append + fall-through)** — 저장분을 제자리(앞쪽)에 두고 `_cursor = _dice.Count`,
  `Peek` 이 커서 소진 후 리스트 전체를 다시 스캔하는 안. 물리적 순서가 `[저장, 새]` 라 **눈으로 본
  것과 실제가 반대**고, 저장분이 커서 뒤라 **죽어도 살아남는다.** 별도 패치가 필요해서 접었다.

  **~~`DiceRecoverEvent` 를 삭제한다~~ 는 중간 결론도 접었다.** "저장분은 방어 전용 예약이라
  자기 행동 순서로 안 돌아온다"는 잘못된 모델에서 나온 것이었다. 실제로는 큐가 그냥 이어지므로
  승격 단계가 필요하고, `Recover()` 는 살아남되 하는 일이 바뀐다(→ `EndBout()` 류로 개명 대상.
  `ReturnCard` 때와 같은 판단 — 이름이 하는 일을 따라가야 한다).

  **함정 넷**
  1. **앞쪽 삽입은 `InsertRange(0, entries)` 한 번으로.** `foreach { Insert(0, e) }` 는 **역순**이다 —
     `Inject` 적재로 주사위가 뒤집혔던 그 함정이 정확히 재발한다. 비대칭 카드가 없으면 관측도 안 된다
  2. **커서 0 은 bout *끝*에 놓는다.** bout 시작에 놓으면 일방 피격 대상이 빠진다 —
     제3자는 남의 bout 에서 맞으므로 `BoutStart` 가 그를 안 건드린다. bout 끝에 두면
     "**bout 밖에서는 모든 커서가 0**"이 불변량이 된다
  3. ~~**`BoutEndEvent` 가 `TargetId` 를 안 챙긴다**~~ — **해소됨.** 이제 `attacker`/`target`
     둘 다 `EndBoutDice()` 를 부른다. 저장분으로 방어하면서 커서가 움직인 캐릭터가 정리에서
     빠지면 안 된다는 것이 이유였다
  4. **`Stored` 상태는 이제 선택사항이다** — 커서가 뒤로 안 가므로 `Consumed` 하나로도 동작한다.
     그래도 따로 두길 권한다. `Peek` 이 `Consumed` 를 받으면 "소비됐는데 왜 집지?"가 되고,
     이건 `Ready` 가 두 뜻을 겸했던 것과 같은 병이다

  ~~**`ResolveUnopposedDice` 가 대상 풀을 peek 하는 부분**이 유일하게 새로 짜는 로직이다~~ —
  **새로 짤 로직은 0이다 (2026-08-05 정정).** `ResolveDiceClash(int idA, int idB)` 가 캐릭터 id
  두 개만 받아 각자 자기 풀을 `Peek`·`Advance` 하고, **"B 가 합 상대인가"를 묻는 코드가 한 줄도 없다**
  (`CombatExecutor.cs:123~159`). 3-1.7 이 `isOwnerA` 를 없앤 결과다. 그래서 `ResolveCombat` 이
  `Peek(targetId)` 로 바꾸기만 하면 그대로 재사용된다. `Attack vs Block` / `Attack vs Evade` 룰은
  값까지 이미 원작대로다. 자세한 형태는 위 "4단계 착수 메모" 참고.

  **합에서 주사위가 모자랄 때는 배선이 0이다.** `ResolveCombat` 이 이미 매 루프 양쪽을 peek 하므로
  (`CombatExecutor.cs:91~92`) `Peek()` 이 저장분까지 이어주기만 하면 `diceB != null` 이 되어
  일방으로 안 빠지고 합이 계속된다.

- ~~**3-1.12. 파괴된 주사위를 `_dice` 에 쌓지 않는다**~~ — ✔ **완료.** 3-1.10 2단계에서
  `EndBout` 의 `ClearDestroyed()` 로 들어갔다. **남은 것은 `_diceById` 읽는 배선 0건 하나**
  (아래 표의 역할 분담은 확정됐는데 읽는 쪽이 아직 없다).
  (2026-08-04 결정, 3-1.10 과 한 덩어리)
  bout 끝마다 `Destroyed` 를 목록에서 제거한다. 그러면 커서가 "시체를 건너뛰는" 전제 위에서
  도는 코드(`Peek`/`Inject`/`DestroyRemaining`)가 통째로 단순해진다. `ResetDiceForNextTurn` 검증 때
  2턴차 `_cursor` 가 `6` 이었던 것도 시체 때문이고, 이걸 넣으면 `2` 가 된다.
  - **`_diceById` 에 남기니까 안전하다는 건 틀린 근거다.** `_dice` 와 **같은 `DiceEntry`** 를 담고
    있고 `DiceRuntime` 은 가변이라, 남겨도 **마지막 상태 하나**만 남는다. 정보량이 같다
  - 진짜 근거는 **이력이 `CombatLogs` 에 있다는 것.** `DiceClashLog` 가 `DiceHandle` +
    **그 순간의** 굴림값을 싣는다 (2026-07-30 "주사위 잔존과 리플레이" 결론 그대로)
  - **`_diceById` 는 오히려 남길 이유가 생겼다.** `DiceHandle` 이 `Owner` + `DiceId` 뿐이라
    로그만으로는 그 주사위의 타입·범위를 못 그린다. 리플레이(`LogDispatcher.DispatchAll`)를 켜면
    조회가 반드시 필요하다. 역할을 이렇게 가른다:

    | | 역할 | 수명 |
    |---|---|---|
    | `_dice` | 지금 쓸 큐 | bout 끝에 시체 제거, 턴 끝에 초기화 |
    | `_diceById` | **불변** 속성 조회용 대장(`Type`/`Min`/`Max`) | 전투 내내 |
    | `CombatLogs` | 이력 (굴림값·결과) | 전투 내내 |

    `_nextDiceId` 가 캐릭터당 한 번만 초기화돼 `DiceId` 가 전투 내내 유일한 것이 이미 이 구도를
    받치고 있다. 9번 전수조사에 "`_diceById` 쓰기 전용" 으로 남겨둔 미결 항목의 답이 이것이다
  - **주의**: `_diceById` 조회로 **`CurrentRoll` 을 읽으면 안 된다.** 덮어써져서 마지막 값뿐이다.
    불변 속성만. 굴림값이 필요하면 로그에서 가져온다
- **3-1.13. 힘이 카운터 주사위를 안 올려줬다 (2026-08-06 발견 → 같은 날 수정·검증 완료)**
  ✔ **`StatusEffects.cs:41`(힘) / `AttackBoostPassive.cs:14` 둘 다 `Counter` 통과하도록 고침.**
  **검증: `DiceUI` 에서 `[id:0 Counter 6~6] [6->9]` 화살표 확인** — 방금 만든 로그 콘솔이
  브레이크포인트 없이 대조군까지 보여줬다.
  `Counter` 는 데미지를 내는 공격형 주사위다(룰 테이블에서 Counter 승리가 `DamageContext` 를 만든다).

  ✔ **`IsOffensive` 확장 메서드도 완료 (2026-08-06, 검증 완료).** 3-1.13 종결.
  `DiceTypeExtensions.IsOffensive(this DiceType) => Attack || Counter`, 자리는 `DiceData.cs` 의
  `DiceType` 옆. 호출부 두 곳은 `!ctx.Dice.Type.IsOffensive()` 한 줄.
  **오용 경고 주석을 지금 달았다** — "`Attack` 만 필요한 자리에는 쓰지 말 것".
  주석 일괄 작업으로 미룰 수 없는 종류다(일괄 작업은 "뭘 하나"고 이건 경고라 헬퍼가
  존재하는 순간부터 필요하다). 근거는 아래 표 밑에 그대로 남겨둔다.

  **핵심 — 같은 `!= DiceType.Attack` 이 네 곳에 있는데 뜻이 두 가지다:**

  | 위치 | 진짜 묻는 것 | Counter |
  |---|---|---|
  | `StatusEffects.cs:41` (힘) | **공격력을 올릴 대상인가** | **포함 ← 고침** |
  | `AttackBoostPassive.cs:14` | 같은 뜻 | **포함 ← 고침** |
  | `CombatExecutor.cs:162` (`ResolveUnopposedDice`) | 상대 없이도 굴리나 | **아니다 — 저장. 그대로 둘 것** |
  | `DicePool.cs:81` (`DiscardRemaining`) | 상대 없으면 소멸하나 | **아니다 — 저장. 그대로 둘 것** |

  뒤 둘은 원작 규칙("블락/회피/**카운터**는 맞붙을 상대가 없으면 저장")대로라 **맞다.**
  - **그래서 `IsOffensive`(`Attack` | `Counter`)로 이름을 붙인다** — `DiceType` 옆(`DiceData.cs`)에
    확장 메서드로. 앞 둘만 그걸 쓰면 **코드 모양부터 달라져서** "네 곳 중복이네" 하고 합칠 수가 없다.
    근거를 강한 순으로:
    1. **원작 규칙에 이름이 있다.** "힘은 **공격 주사위** 위력을 올린다"의 그 범주다.
       지금 코드는 규칙이 아니라 **규칙의 구현**(멤버 나열)을 적고 있다
    2. **enum 이 늘면 조용히 틀린다.** 공격형이 하나 더 생기면 두 줄을 다 찾아 고쳐야 하고,
       놓치면 증상이 **2026-08-06 에 고친 이 버그와 똑같다**
    3. (약함) 네 곳을 잘못 합치는 것 방지 — 추측이다
  - **반대 논거도 있다**: 3-1.7 이 "효과마다 다르므로 타입 검사는 훅 안"으로 정했는데 헬퍼는
    거기 살짝 어긋난다. 나중에 **`Attack` 만** 올리는 효과가 생기면 `IsOffensive` 를 **쓰면 안 되는데**
    있으면 무심코 쓴다. 두 곳이 우연히 같은 조건인 것과 같은 범주인 것은 다르다
  - 이름 주의: `IsAttackType` 은 `DiceType.Attack` 과 헷갈린다. **범주 이름**이어야 한다
  - 마비는 가드가 없어 전 타입에 걸린다. **힘과 마비의 범위가 다른 게 정상이다**
  - 뒤 두 곳은 주석 작업 때 "여기 `Attack` 만인 건 저장 규칙 때문이지 공격형 판정이 아니다"를 달 것

- ~~**3-1.11. `DiceClashLog` 가 보정 전 `CurrentRoll` 을 찍는다**~~ — ✔ **완료 (2026-08-06, 검증 완료).**
  원본을 빼지 않고 **두 쌍**으로 갔다: `BaseRollA/B` + `ModifiedRollA/B`.
  - **`Base`/`Modified` 네이밍이 핵심이다** — 이름이 "둘 중 뭐가 보정 전인지"를 스스로 답한다
  - 값은 `clashCtx` 가 아니라 `ResolveDiceClash` 의 **지역변수**에서 가져온다.
    `ClashContext` 의 두 프로퍼티가 `{ get; set; }` 이라 "판정에 쓰인 값"을 타입이 보장 못 한다
  - `BaseRoll` 을 `entry.Dice.CurrentRoll` 로 읽는 건 안전하다 — 3-1.7 이 **"`CurrentRoll` 은
    `Roll()` 만 쓴다"**고 못박아뒀다. 그 규칙이 이 줄을 받친다
  - 같이 한 것: **`UnopposedLog` 배선**(생산자 0건이었다). `{Handle, TargetId, BaseRoll, ModifiedRoll}`
    로 필드를 다시 잡고 `ResolveUnopposedDice` 의 죽음 가드 뒤·`DamageEvent` 앞에서 `AddLog`.
    `DiceType`/`Advance` 는 그 분기에서 **상수라서 뺐다**(Attack 고정, Destroy 고정) —
    `DiceClashLog` 에 `AdvanceA/B` 가 있는 건 룰 테이블이 세 종류를 다 만들기 때문이다.
    **대칭을 위해 넣는 게 아니라 변하는 것만 싣는다**
  - 비-Attack 분기에는 로그를 안 낸다. 굴리지 않았으니 실을 굴림값이 없고 `DiceConsumedLog` 가
    이미 그 사건을 적는다. 그래서 주사위에 일어날 수 있는 일과 로그가 1:1 이 된다:
    **합 → `DiceClashLog` / 일방 → `UnopposedLog` / 상대 없어 저장 → `DiceConsumedLog`**
  - `Event` 짝은 안 만들었다. `DiceClashLog` 가 이미 이벤트 없이 `AddLog` 로 나가는 선례다.
    Event/Log 1:1 규칙은 **상태를 바꾸는 로그** 얘기고 이 둘은 해석 결과 기록이다
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

### 4. Engine → Data 의존 끊기 — 구 2번 — ✔ **완료 (2026-08-21). 1~4단계 전부**

**`Engine/` 안의 SO 참조가 0건이고, 이제 그 0건을 컴파일러가 지킨다.**
`CharacterData`/`CardData`/`PassiveData` 가 나오는 곳은 주석뿐이고, 그중 일부는 아직
**거짓**이다(위 "거짓이 된 주석" 목록 — 주석 일괄 작업 때 처리).

- ~~`CharacterData` ← `CharacterState`, `CharacterStateBuilder`, `BattleSnapShot`~~ — ✔ **2단계로 해소**
- ~~`PassiveData` ← `CharacterState`, `PassiveFactory`~~ — ✔ **3단계로 해소**

#### 2단계 — ✔ **완료·플레이 검증 완료 (2026-08-20)**

`CharacterModel`(순수, 프로퍼티 16개 전부 `{ get; }`) 신설 + `CharacterData.ToModel()`.
`CharacterState`/`CharacterStateBuilder`/`BattleSnapShot` 이 모델을 받고,
`BattleManager.ExtractData` 가 `List<CharacterModel>` 을 돌려준다.

- **`CharacterState` 의 변환 루프 두 개가 사라졌다.** 카드·패시브 변환이 `ToModel()` 로 이사해서
  `Passives = source.Passives;` / `InitialDeck = source.InitialDeck;` 두 줄이 됐다.
  **선택이 아니라 강제다** — `CharacterState` 에서 SO 를 뺏으면 그 루프는 거기 있을 수가 없다.
  결과적으로 **SO → 모델 변환이 관문 한 곳(`CharacterData.ToModel()`)으로 모였다**
- **`CharacterModel` 생성자는 값 16개를 받는다. SO 를 받으면 안 된다** — 첫 제출이
  `CharacterModel(CharacterData data)` 였는데, 그러면 `Engine/` 에 SO 참조가 하나 **늘어난다**.
  `CardModel` 이 `CardData` 를 안 받는 것과 같은 이유
- **인자가 거의 다 `int` 라 순서를 바꿔도 컴파일이 통과한다.** 그래서 호출부에서
  **named argument** 를 쓴다(`maxHp: _maxHp`). 생산자가 `ToModel()` 한 곳뿐이라 이것으로 완전히 막힌다.
  `init` 프로퍼티는 Unity 6 에서 `IsExternalInit` 심이 필요할 수 있어 접었다
- **`CharacterData` 에 `InitialDeck` 프로퍼티만 남겼다.** artwork 레지스트리가 `CardData.Artwork` 를
  읽어야 하는데 `CardModel` 에는 없기 때문이다(Scene 계층이라 SO 를 봐도 된다).
  `ToModel()` 은 private `_initialDeck` 를 직접 쓰므로 **이 프로퍼티의 유일한 독자가 그 루프**다
- **3단계와 성격이 정반대였다.** 순수 타입 교체라 **컴파일러가 전부 잡아준다** —
  3단계의 `is IStatModifierPassive` 처럼 조용히 false 가 되는 경로가 없었다.
  유일한 조용한 위험이 위의 `int` 인자 순서였고, named argument 로 덮었다

#### 4단계 — ✔ **완료·플레이 검증 완료 (2026-08-21)**. 아래는 설계 근거 기록

`Assets/Scripts/Engine/LOR.Engine.asmdef` 신설.
`noEngineReferences: true` / `autoReferenced: true` / `references: []`.
`Engine/` 115개 파일이 `LOR.Engine.dll` 로 갈라져 나오고, 나머지(`Data`/`Scene`/`UI`)는
`Assembly-CSharp` 에 남는다.

**게이트가 둘이다. 하나는 체크박스로, 하나는 공짜로 딸려온다:**

| 막는 것 | 수단 |
|---|---|
| `Engine` → `UnityEngine` | **No Engine References** 체크박스 |
| `Engine` → `Data`/`Scene`/`UI` | **어셈블리 분리 그 자체** — 아래 |

- **둘째가 이 항목의 제목이다.** `Assembly-CSharp` 가 `LOR.Engine` 을 참조하므로
  (`autoReferenced: true`) 반대 방향은 **순환 참조**가 되고 Unity 가 아예 거부한다.
  즉 `Engine/` 은 `CharacterData` 를 **쓰고 싶어도 쓸 방법이 없다**
- **`Auto Referenced` 를 켜두는 진짜 이유가 이것이다.** 끄면 `Data`/`Scene`/`UI` 가 Engine
  타입을 못 봐 프로젝트가 통째로 안 도는데, 그건 증상이다. 켜야 화살표가 한 방향으로
  **자동** 고정되고, 손으로 걸면 반대로 걸 여지가 생긴다
- **`internal` 이 Engine 에 0건이라 `CS0122` 가 안 났다.** 어셈블리가 갈리면 `internal` 범위가
  바뀐다 — 다음에 다른 폴더를 가를 때 먼저 세어볼 것
- **`Engine/` 에 `namespace` 선언이 0개(전부 global)인데 문제가 안 된다.** 어셈블리만 갈리고
  이름은 안 갈리기 때문이다. 어셈블리 경계와 네임스페이스는 별개다

**검증은 "컴파일 통과" 가 아니라 `rsp` 파일로 했다** (`Library/Bee/artifacts/*.dag/*.rsp`):

| 어셈블리 | `UnityEngine` 참조 |
|---|---|
| `LOR.Engine` | **0건** (135개 참조가 전부 .NET BCL) |
| `Assembly-CSharp` | **134건** |

**이 관측은 대조군이 공짜로 딸려 있다** — 같은 `grep` 이 한쪽에서 134건을 잡으므로
"참조가 없는 것" 과 "`grep` 이 안 걸린 것" 이 갈린다. 브레이크포인트가 두 번 거짓말한 뒤
세운 기준("관측 지점은 두 가설을 갈라주는가로 고른다")을 관측 지점 **고르는 단계**에서 만족한 첫 사례.

**덤 — 엔진이 자기 안에서 완결된다.** `DeterministicRng` 가 `Engine/Support/IRng.cs` 에 있고
진입점 `BattleSnapShot(IEnumerable<CharacterModel>, IEnumerable<CharacterModel>, int seed)` 가
받는 것도 순수 타입 + `int` 시드뿐이다. **`LOR.Engine.dll` 은 Unity 없이 콘솔 앱에서 돈다** —
"리플레이 = 시드 + 입력열" 이 말뿐이 아니라 실제로 성립한다.
- **단 아직 아무도 안 돌려봤다.** 9번 전수조사의 "만들어놓고 배선 0건" 과 같은 자리다.
  실제로 돌리면 지금 **플레이 + 출혈 카운터 + 브레이크포인트**로 하는 검증들이 `Assert` 몇 줄이 된다
  (출혈 스택 = 굴림 카운터가 그대로 단언문이 된다). 별건으로 남겨둔다

#### 3단계 — ✔ **완료·플레이 검증 완료 (2026-08-20)**. 아래는 설계 근거 기록

**결과**: `PassiveModel`(추상) + 서브클래스 4개 신설, `PassiveData` 는 `[SerializeField] amount` +
`ToModel()` 만 남는 껍데기가 됐다. `PassiveFactory` / `PassiveType` / `PassiveEffect.Type` **삭제**.
`CharacterState.Passives` 는 `IReadOnlyList<PassiveModel>`. **`Engine/` 안의 `PassiveData` 참조 0건.**

**실제로 밟은 순서 (계획과 다른 부분이 있다)**
1. 모델 5파일 신설 — 기존 파일은 한 줄도 안 건드린다. 컴파일만 통과하면 끝
2. `PassiveData` 에 `abstract ToModel()` + 4개 구현 **추가만.** SO 의 `Apply` 는 **남겨둔다**
3. `CharacterState` 가 모델 리스트를 **먼저** 만들고 builder 루프가 **그걸** 순회
4. 3-후: SO 에서 `IStatModifierPassive` + `Apply` 제거 (**독자를 옮긴 뒤에**)
5. `CharacterRuntime` 이 `model.CreateEffect(this)` 를 부르고 `PassiveFactory` 삭제
6. `PassiveType` 일체 + SO 의 죽은 `Amount` 접근자 3개 삭제

**함정 — "독자를 먼저 옮기고, 옛 구현은 나중에 지운다".** 2단계에서 SO 의
`IStatModifierPassive` 를 같이 걷어내면 `CharacterState` 의 `passive is IStatModifierPassive` 가
**항상 false** 가 되는데 **컴파일은 통과한다**(`is` 검사라 타입 에러가 안 난다).
증상은 `HpUI` 가 `50/50` 이고, "리팩터링 중이라 그런가" 하고 넘기기 딱 좋다.

**함정 — 3단계 도중에는 대조군이 무정보다.** `CharacterState` 를 반만 고친 상태
(`Passives` 타입은 바꿨는데 `Apply` 루프는 아직 SO 순회)에서도 `HpUI` 는 `70/70` 이 나온다.
SO 의 `Apply` 가 여전히 돌기 때문이다. **그래서 이 단계의 판정 기준은 화면이 아니라
`CharacterState.cs` 안의 `source.Passives` 참조 개수(1건)였다.** 검증이 의미를 갖는 건
4단계 이후다.

**함정 — `[SerializeField] private int amount;` 는 남긴다.** 지운 것은 `public int Amount => amount;`
접근자뿐이다. 필드를 지우면 에셋의 `amount: 3`/`20` 이 날아가고, 증상이 **"패시브는 도는데
효과가 0"** 이라 `amount` 를 안 채웠을 때와 완전히 같아진다.

**`PassiveData.Name` 은 남겼다.** 독자 0건이지만 이번 리팩터링이 만든 잔재가 아니고
에셋에 실제 값이 들어 있다(`passiveName: AttackBoost`). 패시브 UI 를 만들 때 쓸 자리다.

#### 2/3단계의 경계 — **결정: 3단계 먼저, B안** (2026-08-19)

계획이 2/3단계를 갈라놨는데 실제로는 엉켜 있었다. `CharacterState.Passives` 가
`IReadOnlyList<PassiveData>` 고 생성자가 `passive is IStatModifierPassive` 로 SO 를 직접
부른다(`CharacterState.cs:41`). **2단계만 하면 `CharacterModel` 이 `List<PassiveData>` 를 들고
있어 누수가 그대로다.** 그래서 **3단계를 먼저** 한다 — `PassiveModel` 이 서면 `CharacterModel` 은
필드 복사만 남아, 큰 덩어리가 "설계가 필요한 부분"과 "기계적인 부분"으로 갈린다.

관문 패턴 자체는 `CardData.ToModel()` 선례가 이미 정해뒀다. `CardModel` 이 `DiceData` 를 그대로
들고 가는 것이 문제가 안 되는 이유는 **`DiceData` 가 Engine 소속 순수 클래스**이기 때문이고
(`Engine/Dice/DiceData.cs`), `PassiveData` 는 SO 라 그 면제가 안 된다.

**발견 1 — 패시브 파이프라인은 한 번도 안 돌았다.** `Ally01`/`Enemy01` 둘 다 `_passives: []` 이고
`PassiveData` 에셋이 0개다. `.asset` 마이그레이션 위험이 0이라 **구조를 바꾸기에 제일 싼 시점**이고,
동시에 **플레이 검증을 하려면 에셋을 먼저 만들어야 한다**(`Evade`/`Counter` 카드 때와 같은 상황).

**발견 2 — `PassiveEffect.Type` 은 쓰기 전용이다.** 생성자에서 받아 노출하는데 읽는 곳이 0건이다
(`StatusEffectRuntime.Type` 이 `_effectMap` 키로 쓰이는 것과 대비 — `CharacterRuntime.cs:414`).
따라서 **`PassiveType` enum 의 유일한 독자는 `PassiveFactory.Create` 의 `switch` 하나**고,
그 enum 이 나르는 정보는 "어느 서브클래스인가" = **타입 자체가 이미 갖고 있는 것의 복사본**이다.
`DefenderId`/`idB` 와 같은 종류의 중복이고, 다른 점은 여기선 그게 디스패치 태그로 쓰인다는 것뿐이다.

**채택 — B안 (다형성 디스패치)**
```
PassiveModel (abstract) { abstract PassiveEffect CreateEffect(CharacterRuntime owner); }
  ├ AttackBoostModel  → new AttackBoostPassive(owner, Amount)
  ├ SpeedSlotModel    → new SpeedSlotPassive(owner)
  └ EmotionOnAttackModel / MaxHpBoostModel : IStatModifierPassive → null
PassiveFactory / PassiveType / PassiveEffect.Type 삭제
CharacterRuntime: var passive = model.CreateEffect(this);
```
- 다운캐스트(`((AttackBoostData)data).Amount`)와 `_ => throw` 가 소멸한다. 후자는 `abstract` 라
  **컴파일러가 강제**한다. 새 패시브 하나에 고칠 곳이 넷(SO/Model/enum/switch) → **둘**(SO/Model)
- 실익의 핵심은 **"enum 이 늘면 조용히 틀린다"를 구조적으로 못 나게 하는 것.**
  3-1.13(힘이 `Counter` 를 빼먹은 것)이 그 병이었고 `IsOffensive` 는 완화였지 방지가 아니었다
- **접은 A안**: 현행 구조 유지 + 모델만 끼우기. 변경은 최소지만 위 넷이 그대로 남는다
- **반론도 남겨둔다**: "모델은 순수 데이터여야" → `PassiveModel` 도 `PassiveEffect` 도 둘 다 Engine 이라
  계층 위반이 아니고, 스탯 수정형은 이미 `Apply(builder)` 라는 행동을 갖고 있다.
  "패시브 모델은 순수 데이터"라는 전제가 성립한 적이 없다
- **`null` 반환은 B안에도 남는다.** 스탯 수정형은 런타임 훅이 없다 — 실재하는 개념이고,
  지금 factory 주석에 적힌 그 사실이 **클래스 자신에게로** 옮겨간다

**함정 — 변환 순서 (실제로 한 번 걸렸다).** `CharacterState` 생성자가 `source.Passives`(SO)를
**두 번** 썼다: `Apply` 순회와 리스트 복사. **모델 리스트를 먼저 만들고 두 곳 다 그걸 순회해야**
SO 참조가 0이 된다. 첫 제출에서 리스트 복사 쪽만 바뀌고 `Apply` 루프가 SO 순회로 남았다 —
**"짝인데 한쪽만" 실수가 미리 경고를 적어둔 상태에서 또 났다.**

**`.asset` 은 안 깨진다.** `[SerializeField] amount` 필드명과 SO 클래스명을 안 건드리므로
`FormerlySerializedAs` 불필요 — 애초에 에셋이 0개라 더더욱. (`AttackBoostData` 만 이름이 `...Data` 이고
나머지 셋은 `...PassiveData` 인데, **개명은 이번 작업과 섞지 말 것**)

**3단계가 끝나면 남는 누수는 `CharacterData` 하나다.** `CharacterStateBuilder` 는 여전히 SO 를
받는데 그건 2단계 몫이다.

#### 검증용 패시브 에셋 (3단계 착수 전에 만든다 — 사용자 결정 2026-08-19)

**두 개면 두 경로를 다 덮는다.** 나머지 둘은 각각의 형제라 같은 코드 경로를 지난다.

| 에셋 | 경로 | 관측 창구 |
|---|---|---|
| `MaxHpBoost` (+20) | **빌드 타임** — `IStatModifierPassive` → `CharacterStateBuilder` | `HpUI` 가 `50/50` → `70/70` |
| `AttackBoost` (+3) | **런타임 훅** — `PassiveEffect.OnModifyRoll` | `DiceUI` 의 `[6->9]` 화살표 |

- 붙일 대상은 **`Ally01`** (characterId 0 — `HpUI`/`StatusUI`/`DiceUI` 가 전부 여기 묶여 있다)
- **`AttackBoost` 검증에는 `Counter` 카드(Counter 6~6)를 쓴다.** 고정값이라 `[6->9]` 가 확정이고,
  **`Evade` 카드(Evade 6~6)가 그대로 대조군**이다 — `IsOffensive` 가 아니므로 `[6->6]` 이어야 한다.
  둘이 서로를 갈라주므로 "패시브가 도는가"와 "전 타입에 붙는가"가 한 번에 구분된다.
  주사위를 굴리려면 합이어야 한다(비-Attack 은 일방에서 저장된다) — `Combo`(1~3)에 붙이면 확정 승리
- **`SpeedSlot` 은 검증에 쓰지 말 것.** 감정 레벨이 필요해 세팅이 번거로운 데다,
  `_activeSpeedSlotCount` 가 여전히 쓰기 전용이라(9번) 슬롯 수가 실제로 안 늘어난다
- **함정: `amount` 는 `int` 라 기본값이 0이다.** 인스펙터에서 안 채우면 패시브가 정상적으로 도는데
  효과가 0이라 **증상이 "패시브가 안 돈다"와 똑같이 보인다.** `DiceData.Type` 이 조용히 `Attack` 이던
  `Execute` 건과 같은 종류다
- **만들고 나서 `File > Save Project`.** ScriptableObject 편집은 메모리에서 dirty 로만 있고
  에디터 안에서는 정상으로 보인다. `git status` 로 `Ally01.asset` 의 `_passives` 가 실제로 채워졌는지
  확인할 것 — `Evade`/`Counter` 가 `_dices: []` 인 채로 커밋될 뻔했다
- **대조군을 지금(고치기 전에) 돌려서 통과를 확인한다.** 이게 "에셋 먼저"의 이유다.
  나중에 만들면 새 코드와 새 에셋이 동시에 들어와 뭐가 원인인지 못 가른다

### 5. 카드 뽑기 장수를 상수 → 변수로 — 구 3번
`Engine/Events/TurnStartEvent.cs` 가 `new DrawCardEvent(CharacterId, 1)` 로 고정.
"다음 턴 카드 +n" 같은 효과를 만들 때 두 개가 필요해진다:
- 기본 장수 → `CharacterData` → `CharacterState` → `CharacterRuntime`
- 일시 보정 → `StatusEffectRuntime` 이 이미 `OnTurnStart` 훅과 만료 처리를 갖고 있으므로 그쪽이 적합

### 6. 잔재 정리 — 구 4번 — ✔ **완료 (2026-08-19)**
- ~~`UI/Slot/SpeedSlotUI.cs`~~ — **삭제.** 씬·프리팹 어디에도 안 붙어 있었다(guid 참조 0건)
- ~~`TurnUI`~~ — **필드 2개가 아니라 파일 통째로 삭제.** 아래 참고
- ~~`SlotDebugPanel.cs:57` 의 진단용 `Debug.Log("[BoutStart] ...")`~~ — 삭제. `DiceUI` 가
  `BoutStartLog` 을 구독해 같은 내용을 bout 구분선으로 찍으므로 **정보 손실 0**
- ~~`PlayerActionInput.cs:13,18`~~ — 목록에 없던 진단용 `Debug.Log` 2건. 같이 삭제
- ~~`BattleRuntime.HasEvents`~~ — **이미 지워져 있었다.** 목록만 헌 것이었다(9번의 `SlotRuntimeMap` 도 동일)
- ~~`BattleInput.cs` / `BattleResult.cs` / `BattleRuntime.Start`~~ — 2026-07-26 삭제 완료
- 덤: `SpeedSlotPassvieData` → **`SpeedSlotPassiveData`** 오타 수정("파일명 = 클래스명" 규칙 위반).
  이 스크립트를 쓰는 `.asset` 이 하나도 없어서 공짜였다 — **에셋이 생긴 뒤였으면 리셋 위험이 붙는다**

**`TurnUI` 는 필드가 아니라 파일 전체가 죽어 있었다.** 이 목록이 "필드 2개가 안 쓰인다"로 적혀 있어서
진단이 거기서 멈췄다. 필드를 지우고 나서야 남은 게 껍데기라는 게 보였다:
- 씬에 `TurnUI` 인스턴스가 없다 (`Main.unity` 에 guid `62c0ea8b...` 가 0건)
- End Turn 버튼은 `BattleManager.EndTurn` 을 **직접** 부른다(`Main.unity:3199~3202`).
  `onClickEndButton` 을 부르는 바인딩은 씬 전체에 0건이었다
- 규칙 위반이던 `onClickEndButton`(메서드는 PascalCase)도 파일과 함께 사라졌다.
  **이름만 바꿨으면 컴파일은 통과하는데 버튼이 조용히 아무것도 안 하게 된다** — `UnityEvent` 가
  메서드 이름을 문자열로 들고 있어서, 개명은 인스펙터 재바인딩과 짝이어야 한다

**UI 스크립트의 죽음은 `grep` 으로 판단할 수 없다.** MonoBehaviour 는 코드가 아니라 **씬이** 부르고,
`UnityEvent` 는 메서드 이름을 **문자열로** 들고 있어 코드 검색에 안 잡힌다.
`.meta` 의 guid 로 `.unity`/`.prefab` 을 뒤지는 것이 유일한 확인 방법이다.
(`.cs` 참조 0건만 보고 판단하지 말라는 `Bout.cs` 기준의 UI 판이다)

**턴 표시 UI 를 만들 때는 새로 짠다.** `HpUI`/`StatusUI` 처럼 `TurnStartLog` 을 구독하는 컴포넌트가
맞는 형태고, 껍데기를 남겨둬도 그때 재사용할 게 없었다.

### 7. `Step()` 재귀 구조 (급하지 않음) — 구 5번
`BattleRuntime.EnqueueEvent` 가 enqueue 직후 `Step()` 을 호출하고, `Step()` 은 다른 곳에서
호출되지 않는다. 따라서 **큐에 원소가 2개 이상 쌓이지 않으며**, 사실상 `ev.Apply(this)` 와
등가다. 이벤트는 깊이 우선(DFS)으로 즉시 처리된다.

- 의도(파생 이벤트가 전부 처리됨)는 달성되나 메커니즘은 큐 배수가 아니라 재귀
- `HasEvents` 는 아무도 쓰지 않고 외부에서 항상 `false` — 배수 루프를 염두에 뒀던 흔적
- 부작용: 전투 종료 후 남은 `DeathEvent` 가 처리되면 `BattleEndLog` 중복 가능
- 고치면 DFS → BFS 로 실행 순서가 바뀌므로 전투 로그 비교 검증 필요

### 8. `BoutGraph` — 액션 등록/취소 정합성 (2026-07-31 발견)

**원작 규칙 (사용자 확인 완료)**
- 적 액션은 턴 시작에 고정. 슬롯에 행동이 있거나, 그 턴 내내 없거나 둘 중 하나
- **조건 A (상호)**: 적이 노리는 내 슬롯에 행동을 걸고 그 적 슬롯을 지목. **속도 무관**
- **조건 B (속도)**: 내 슬롯 속도 > 적 슬롯 속도면 **무조건** 합. 상대가 누굴 노리든 무관
- **탈취**: 조건 B 로 다른 슬롯이 같은 대상에 걸면 기존 합이 풀리고 새 합이 된다.
  풀린 쪽은 **합 후보로 복귀**. 후보는 Tab 으로 순환 (Tab 은 미구현)
- 슬롯 덮어쓰기 가능. 우클릭 취소도 가능 (둘 다 합법)

**전투 규모는 최대 5대5 (2026-08-04, 사용자 확인)** — 양 진영 5명까지.
**합 자체는 여전히 1:1 이다.** `BoutGraph.edges`(`Dictionary<SpeedSlot, SpeedSlot>`),
`RunQueue` 의 `visited`(슬롯당 bout 1개), `ResolveCombat(a, b, ...)`, `BoutEndEvent` 전부 구조 변경 없음.
위 "탈취" 규칙이 그 1:1 을 유지하는 장치다 — 아군 둘이 같은 적을 노려도 한쪽만 합을 가져가고
밀려난 쪽은 후보로 복귀한다.

- 따라서 `BoutEndEvent` 가 `{AttackerId, TargetId}` 만 정리하는 것은 5대5 에서도 **완전하다**
  (`DefenderId` 는 항상 `TargetId` 와 같거나 null. 파티 크기와 무관한 성질이다)
- ~~`DefenderId` 를 `bool WasClash` 로 축약하지 않고 `int?` 로 남긴 이유는 "로그가 이벤트보다
  정보가 적으면 안 된다"는 판단이다(사용자)~~ — **2026-08-06 뒤집힘. `bool WasClash` 로 간다 (B안).**
  옛 문장은 **이벤트→로그로 갈 때 정보를 떨구지 말라**는 규칙이었지 `int?` 가 옳은 타입이라는
  주장이 아니었다. **둘 다** `bool WasClash` 로 하면 그 규칙은 그대로 지켜진다.
  즉 결정을 뒤집은 게 아니라 그 결정이 답한 적 없는 질문이다
  - **`DefenderId` 가 나르는 정보는 정확히 1비트다** — "합이었나". `int` 부분은 항상
    `TargetId` 의 복사본이라 0비트다 (`RunQueue` 가 `opponent = ActionBySlot[targetSlot]` 로
    잡으므로 `B.SourceSlot == targetSlot`)
  - **`idB` 가 정확히 같은 물건이었고 실제로 버그를 냈다.** 4단계 항목 참고 —
    "`idB`(`int?`)가 나르던 정보는 캐릭터가 아니라 **'상대가 bout 참가자인가'라는 bool**".
    `null` 이 "그런 캐릭터가 없다"와 "합이 아니다" 두 뜻을 겸해서 `ResolveCombat` 이
    앞의 뜻으로 읽었다. **`DefenderId` 는 그 `idB` 의 살아남은 쌍둥이다** —
    같은 `BoutStartEvent` 에서 나온 같은 식이고, 4단계가 `ResolveCombat` 쪽만 정리했다
  - **`int?` 는 "방어자가 대상이 아닐 수도 있다"고 거짓말한다.** 5대5 에서도 합은 1:1 이
    유지되므로(탈취 규칙이 그 장치) 그 상태는 영영 안 생긴다. **일어날 수 없는 상태를
    표현할 수 있는 타입은 다음 사람이 `if (DefenderId != TargetId)` 를 쓰게 만든다**
  - 읽는 쪽도 달라진다. `log.WasClash` 는 그 자체로 읽히지만 `log.DefenderId.HasValue` 는
    "방어자가 있나"로 읽혀서 **불변량을 아는 사람만** 합 여부로 해석한다
  - **비트 자체는 지울 수 없다.** 합이었는지는 다른 로그로 역산이 안 된다(한 bout 안에
    합과 일방이 둘 다 들어간다). 그래서 "필드 삭제"가 아니라 "타입 교체"다
- **5대5 에서 실제로 드러날 것들**:
  - **제3자 일방 피격이 흔해진다.** 3-1.10 의 4단계(`ResolveCombat` 이 대상 풀을 peek)가
    1대1 에선 드문 경우지만 5대5 에선 일상이 된다 — 우선순위가 올라간다
  - `RunQueue` 의 `!hasEdge` 폴백. `SpeedSlot` 이 struct 라 `TryGetValue` 실패 시 `default`
    (= 0번 캐릭터 슬롯)가 나온다. 대상이 늘수록 "엉뚱한 0번을 때리는" 오작동 여지가 커진다
  - UI 는 **코드 변경 불필요**(`HpUI`/`StaggerUI`/`StatusUI`/`EnergyUI`/`EmotionUI` 전부
    `characterId` 가 `[SerializeField]`)지만 씬에 인스턴스를 10벌 놓아야 한다.
    `CardHandUI` 는 캐릭터마다 손패가 따로라 "선택된 캐릭터 것만 보이기" 같은 설계가 따로 필요
  - 아래 5번의 "플레이어 진영 = Ally 하드코딩" 우려가 그만큼 커진다

**할 일**

- ~~1. `CancelAction` 이 `actionBySlot` 에서 안 지운다~~ — **완료 (2026-07-31).**
  `RemoveFromActionBySlot` 신설, `CancelAction` **맨 앞**에 배치(`ReevaluateAffectedSlots` 가
  `actionBySlot` 을 읽으므로 그보다 앞이어야 한다. "첫 줄 이후로는 이 액션이 없는 세계" 불변량).
  `ReferenceEquals` 로 동일 인스턴스 확인 후에만 제거 — `FlushExpired` 의 `_effectMap` 처리와 같은 선례.
- ~~2. `RegisterAction` 의 덮어쓰기가 정리를 안 한다~~ — **완료 (2026-07-31).**
  맨 앞에서 기존 액션을 찾아 `CancelAction` 을 태운 뒤 등록. 원작이 우클릭 취소와 덮어쓰기를 둘 다
  허용하므로 두 입력이 같은 코드를 지나 결과가 어긋날 수 없다.
- **3. 조건 A 로 성립한 합이 조건 B 에 탈취당하면 후보 목록에서 사라진다** (남음) —
  `AddInterceptCandidate` 가 `TryBuildInterceptClash` 안에서만 불린다(`BoutGraph.cs:89`).
  범위는 **내 슬롯이 적 슬롯보다 느려 조건 A 로만 붙은 경우** 한정. 규칙의 "풀린 합은 후보로 복귀"가
  이 경우에 성립하지 않는다. Tab 순환을 만들 때 같이 볼 것.
- ~~4. `ReevaluateAffectedSlots` 가 `SourceSlot` 쪽을 안 본다~~ — **논점 아님으로 판명.**
  아래 5번(인터셉트 게이트)이 들어가면 인터셉트 합은 항상 플레이어가 만든 것이고,
  그 슬롯을 덮어쓰면 `UpdateRelationsFor(새 액션)` 이 같은 조건을 재평가해 **스스로 복구한다.**
  복구가 안 되는 경우는 적이 만든 합뿐인데 그게 애초에 없어진다.
- **5. `TryBuildInterceptClash` 가 적 액션에도 적용된다 + 팀 검사가 없다** (승격) —
  **조건 B 는 플레이어 전용이다**(사용자 확인). 적이 더 빠르고 내 슬롯을 노려도, 내가 딴 데를 보면
  **합이 아니라 일방 피격**이다. 그런데 `UpdateRelationsFor` 는 `RegisterAction` 안에 있어
  적 액션 등록 때도 돌고, 그때 인터셉트가 적 쪽에서 성립해버린다.
  - 조건 A(`TryBuildDirectClash`)는 대칭이므로 게이트 불필요. **`TryBuildInterceptClash` 하나만.**
  - **실제 흐름에서는 아직 도달하지 않는다** — 적 액션은 턴 시작에 전부 등록되고 그때는
    플레이어 액션이 없어 첫 줄(`actionBySlot.ContainsKey(target)`)에서 return 된다.
    지금 문제로 보이는 건 적 AI 가 없어 적 액션을 손으로 나중에 넣기 때문(테스트 아티팩트).
  - ~~고치려면 `BoutGraph` 가 팀을 알아야 한다. `SpeedSlotRuntime` 이 `Team` 을 갖게 하는 쪽을 권한다~~
    — **2026-08-22 사용자 결정으로 뒤집힘. `BoutGraph` 는 팀을 몰라야 한다.**
    - **원칙 (사용자): "`BoutGraph` 는 멍청해야 한다."** 합과 일방을 나타내는 **그래프**일 뿐이고,
      "이 공격이 상대편을 향한 게 맞나" 같은 판단과 그로 인한 버그 방지는 **상위 계층의 몫**이다.
      계층 침범을 막는다는 이유도 있다
    - 그러면 팀 대신 **"이 액션이 인터셉트를 시도해도 되는가" 라는 비트**를 상위가 넘긴다
      (예: `RegisterAction(action, bool canIntercept)`). `BoutGraph` 는 규칙을 모르고 시키는 대로만 한다
    - **`DefenderId` → `bool WasClash` 와 같은 모양이다** — 정체(팀/캐릭터)를 넘기지 말고
      **실제로 쓰이는 비트**를 넘긴다. 정체를 넘기면 받는 쪽이 규칙을 알아야 해진다
    - ✔ **그래서 같은 날 `ISpeedLookup`(`int GetSpeed(SpeedSlot)`)으로 좁혔다.**
      좁히기를 막던 유일한 반대 논거가 이 결정이었다. 이제 `BoutGraph` 는 `Roll()` 에도
      `Slot` 에도 손이 안 닿는다 — **"멍청해야 한다" 가 규율이 아니라 컴파일 에러다**
  - **주의**: "플레이어 진영 = Ally" 를 코드에 박는 것이다. 자동 전투·관전이 생기면
    "지금 배치하는 쪽"으로 바뀌어야 한다.
  - **적 AI 를 붙일 때 같이 하는 게 맞다.** 지금 넣으면 검증 수단이 손으로 적 액션을 넣는 것뿐이라
    또 아티팩트를 본다.

**미해결 질문**: 조건 A 가 이미 성립한 조건 B 합을 **탈취할 수 있나?**
`TryBuildDirectClash` 첫 줄이 `if (edges.ContainsKey(target)) return;` 이라(`BoutGraph.cs:117`) 지금은 못 뺏는다.
3번을 어떻게 고칠지가 이 답에 달려 있다.

### 8-B. 카드 배치 흐름 (2026-07-31 완료)

원작대로 **카드가 슬롯에 올라가면 손에서 빠지고, 우클릭 취소나 덮어쓰기로 손에 돌아온다.**

- `ActionRegisteredEvent` — 맨 앞에서 기존 액션의 카드를 `ReturnCardEvent` 로 돌려보낸 뒤
  등록하고 `UseCardEvent` 를 낸다. **옛 액션 조회는 `RegisterAction` 앞**이어야 한다(뒤면 덮어써져 못 읽음).
  `UseCardEvent` 는 `AddLog` 앞 — 상태 먼저, 로그 나중.
- `ActionCancelledEvent` — `CancelAction` → `ReturnCardEvent` → `AddLog`.
- `ReturnCardEvent` / `ReturnCardLog` 신설. **이름이 "취소"에 묶이면 안 된다** — 취소와 덮어쓰기
  두 곳에서 나가므로 목적지(손)만 말하는 `Return` 을 골랐다.
- `CardZone.Remove` 가 `bool` 반환으로. `UseCard`/`DiscardCard`/`ExileCard`/`ReturnCard` 전부
  **제거가 성공했을 때만** 목적지에 넣는다. 이전에는 손에 없는 카드도 `_used` 에 복제됐다.
- 우클릭 취소 배선: `SlotDebugItem` 이 `IPointerClickHandler` 를 구현하고 `Right` 만 통과시킨다.
  Unity `Button` 은 좌클릭만 처리하므로 충돌 없다. `PlayerActionInput.CancelSlot` 에 `Runtime` null 가드.

**검증 지표는 손패 총 장수다.** 등록·취소·덮어쓰기를 반복해도 보존돼야 한다. 개별 동작이 맞아 보여도
어딘가 새면 장수로 드러난다(실제로 `ReturnCard` 가 `_hand.Add` 대신 `_hand.Remove` 를 부르던 것을 이걸로 잡았다).

**부작용**: **한 장으로 두 슬롯 걸기가 이제 불가능하다.** 그동안의 검증 세팅이 전부 여기 기대고 있었다.
슬롯 2개를 채우려면 손에 2장이 필요한데 드로우가 턴당 1장이라, 5번(드로우 장수 변수화)이 앞당겨질 수 있다.

**남은 구멍**: 아군 카드를 적 슬롯에 걸면 `UseCardEvent` 의 캐릭터가 슬롯 주인이라 **적 손에서 빼려다
실패하고 적 `_used` 에 들어간다.** 원작에선 불가능한 조작이므로 `RegisterToSlot` 에서 막아야 하지만,
지금 막으면 적 액션을 넣을 방법이 없어져 검증이 불가능하다. 적 AI 와 함께 처리.

### 9. 배선 누락 전수조사 (2026-07-31 제안)

**오늘 하루에 "만들어놓고 그 자리에서 안 부름"을 일곱 건 만났다** — 전부 검증하다 우연히 걸린 것이지
찾아서 잡은 게 아니다:

| 대상 | 증상 |
|---|---|
| `DicePool.Add` | 호출자 0건, 적재가 `Inject` 로 되어 있어 **주사위 역순** (수정 완료) |
| `CharacterRuntime.ResetDiceForNextTurn` | 호출자 0건, 턴 끝 정리가 통째로 안 돎 (수정 완료) |
| `CharacterRuntime._diceById` | 쓰기 전용. 읽는 곳 0건 (→ **3-1.12 에서 용도 확정: 불변 속성 조회용 대장**) |
| `CharacterRuntime._activeSpeedSlotCount` | 쓰기 전용. **→ 2026-08-22 삭제로 닫힘.** 배선해봤더니 개념 자체가 틀렸다. 아래 참고 |
| `PlayerActionInput.CancelSlot` | 호출자 0건 — 우클릭 취소 입력이 없다 |
| `UseCardEvent` | 생산자 0건 — **카드가 손에서 안 빠진다** |
| `BoutGraph.interceptCandidates` | UI 표시(개수)만 있고 Tab 순환 미구현 |
| `DiceRuntime.Use()` / `DiceState.Used` | **여덟 번째 건 (2026-08-05).** `ToAdvanceEvent` 가 `Reuse` 에 null 을 반환해 이벤트가 안 나가고, 그 아래 `Advance(Reuse)` / `Use()` / `Used` / `DestroyUsed()` 가 전부 도달 불가. → 3-1.10 **5단계** |

엔진을 두껍게 만들고 배선을 나중으로 미룬 시기의 흔적으로 보인다.

#### `_activeSpeedSlotCount` — 개수 배선은 접었다. 남은 것은 개명 (2026-08-22 확정)

**⚠ 이 절의 "풀 + 활성 개수" 모델은 폐기됐다.** 2026-08-21 에 배선했다가 다음날 봉인 규칙을
플레이로 확인하고 되돌렸다. 아래 **"봉인 규칙 확정"** 이 결론이고, 이 절에서 살아남은 것은
**`SpeedSlots` → `SpeedSlotPool` 개명 하나**다. 아래 소비처 표의 2·3행도 그때 뒤집혔다.
**배선해본 것 자체는 소득이었다** — 개수 모델을 실제로 코드에 넣어봐서 두 축이 갈렸다는 게 드러났다.

**옛 설명이 부정확했다** — "감정 레벨이 슬롯 수를 못 올림" 은 틀렸다. `SetSpeedSlotCount` 의
`while (_speedSlots.Count < count)` 가 리스트를 실제로 늘린다. **못 하는 것은 줄이는 쪽이다.**
`_speedSlots` 는 파괴·재생성 없는 **풀**이고 `_activeSpeedSlotCount` 가 "그중 이번 턴에 쓰는 개수" 인데,
소비처가 전부 리스트 전체를 순회해서 그 개념이 실재하지 않는다.

**소비처 4곳이 원하는 것이 서로 다르다. "전부 활성분으로" 가 아니다:**

| 소비처 | 원하는 것 |
|---|---|
| `BattleRuntime.cs:55` (`_slotRuntimeMap` 채우기) | **풀 전체** — 봉인 슬롯도 신원 조회는 돼야 한다 |
| `BattleRuntime.cs:67` (`RollSpeedDice`) | **활성분만** — 봉인된 슬롯에 속도를 굴리면 안 된다 |
| `SlotDebugPanel.cs:68` (UI) | **활성분만** |
| `CombatExecutor.cs:30` (`graph.SlotRuntime[slot].Speed`) | map 경유라 첫 줄에 의존 |

**형태**: `SpeedSlots` → **`SpeedSlotPool`** 로 개명(전체 풀) + **`ActiveSpeedSlotCount`** 프로퍼티 신설.
소비처 2·3은 `for (int i = 0; i < ActiveSpeedSlotCount; i++)` 인덱스 순회(할당 0).
- **개명이 이 안의 절반이다.** 이름을 두면 나중에 `foreach (var slot in SpeedSlots)` 가
  봉인 슬롯까지 **조용히** 포함시킨다. 개명하면 컴파일러가 기존 3곳을 전부 에러로 띄워
  "여긴 풀인가 활성분인가" 를 하나씩 다시 묻게 만든다 — `DiceEntry` 대신 `DiceInfo` 를 내보낸 것과 같은 수단
- 접은 대안: `SpeedSlots` 를 활성분 view 로 두고 풀을 `SpeedSlotPool` 로. 읽기는 낫지만
  view 타입이나 LINQ 가 필요해 더 비싸다

~~**⚠ 별건으로 딸린 버그 — `_slotRuntimeMap` 이 갱신되지 않는다.**~~
✔ **해소 (2026-08-22). 고친 게 아니라 맵을 없앴다.**
- 증상이었던 것: `BattleRuntime` 생성자에서 **딱 한 번** 채우는데 `SpeedSlotPassive` 는
  `OnTurnStart` 에서 슬롯을 늘린다. 전투 시작 후 늘어난 슬롯은 map 에 영영 안 들어가고
  `CombatExecutor.cs:30` 이 그 슬롯을 못 찾았다
- **조회가 계산 가능하다는 것이 열쇠였다.** `SpeedSlot` 은 `(CharacterId, SlotIndex)` 이고
  **풀의 위치 = `SlotIndex`** 이므로
  `GetCharacterRuntime(slot.CharacterId).SpeedSlotPool[slot.SlotIndex]` 한 줄이면 된다.
  **캐시가 없으면 낡을 일도 없다** — "언제 다시 동기화하나" 라는 질문 자체가 사라졌다
- 형태: `ISlotLookup` 인터페이스 신설, `BattleRuntime` 이 구현(`IEventSink` 와 같은 패턴으로
  생성자에서 `this` 를 넘긴다). `BoutGraph` 는 사전 대신 이걸 받고,
  `CombatExecutor` 는 이미 `BattleRuntime` 을 들고 있어 **직접 부른다** —
  그래서 `BoutGraph.SlotRuntime` 공개 프로퍼티가 통째로 사라졌다
- **⚠ 새 불변량: "풀의 위치 = `SlotIndex`" 가 이제 조회의 근거다.** 지금까지는 슬롯을
  *만들 때만* 쓰이던 성질인데 이제 *찾을 때도* 쓰인다. 깨지면 엉뚱한 슬롯의 속도를 읽고,
  증상이 "행동 순서가 이상함" 으로 보여 원인을 정렬 쪽에서 찾게 된다.
  **`SpeedSlotPool` 을 정렬하거나 원소를 제거하면 깨진다**
- **함정 (실제로 겪음)**: 생성자에서 인자를 받아놓고 **필드에 대입하는 줄을 빠뜨렸다.**
  컴파일은 통과하고, **조건 B(인터셉트)를 실제로 걸어야만** `NullReferenceException` 이 난다.
  평소 전투는 멀쩡히 돌아서 "회귀 통과" 로 보인다 — 검증에 인터셉트가 반드시 들어가야 하는 이유

#### 봉인 규칙 확정 (2026-08-22, 사용자 플레이 확인)

**⭑ 봉인은 항상 "다음 턴" 부터 적용된다 (2026-08-22 사용자 추가).**
거는 턴은 정상이고 **다음 턴부터** 슬롯 하나를 못 쓴다. 이것이 설계를 크게 단순화한다:

- **이번 턴의 액션은 손댈 필요가 없다.** 해석 도중에 봉인이 걸려도 그 슬롯에 올라간 카드는
  그대로 수행된다 — "턴 중 봉인 시 등록된 액션 처리" 라는 문제가 **존재하지 않는다**
- **3-2 Delay(발효 지연)와 정확히 같은 모양이다.** `StatusEffectRuntime` 이 이미
  `Stack` / `PendingStack` 두 칸을 갖고 `TickTurnEnd` 가 대기분을 승격시킨다
  (2026-07-27 완료). **"다음 턴부터" 를 위해 새 기계를 만들 필요가 없다**
- **대상 선정도 다음 턴에 하는 것으로 보인다 (파생 — 확인 필요).** 속도는 매 턴 다시 굴려지므로
  거는 시점에 최속이던 슬롯이 다음 턴에도 최속이라는 보장이 없다. 그런데 관측은
  **"항상 가장 빠른 슬롯이 잠긴다"** 였다. 거는 시점에 정했다면 다음 턴엔 최속이 아닌 슬롯이
  잠긴 것처럼 보였을 것이다
- **순서가 이미 맞다.** `StartTurn()` 이 `RollSpeedDice()` → `TurnStartEvent` → `TriggerTurnStart()`
  순이므로, **턴 시작 훅은 이미 굴려진 속도를 본다.** 훅에서 "가장 빠른 비봉인 슬롯" 을 골라
  잠그면 배선 추가가 0이다
- **검증이 싸진다.** 지연 적용이라 `BattleManager` 에 임시 `DebugAddSeal()` 버튼 하나면
  "다음 턴에 슬롯 하나가 깨진 채 맨 왼쪽에 뜨는가" 를 볼 수 있다.
  **발동 조건(주사위/카드 에셋)을 먼저 만들 필요가 없다**


원작을 직접 돌려서 확인한 것이다. **개수 모델이 여기서 죽었다.**

| 관측 | 설계 결론 |
|---|---|
| 봉인 슬롯이 **깨진 모습으로 계속 보인다** | 슬롯은 사라지지 않는다 → **`IsSealed` 플래그**. `int` 로는 "존재하는데 못 쓴다" 를 표현할 수 없다 |
| **항상 맨 왼쪽**을 차지한다 | **표시 규칙**이다. 속도 정렬의 부수효과가 아니다 — 유진(영구 봉인)은 속도가 없는데도 자리가 고정이다 |
| "지정 불가능" = **거기서 카드만 못 낸다** | 게이트는 `SourceSlot` 한쪽뿐. **타겟으로는 양쪽 다 지정 가능**하다 |
| 봉인 대상은 맞은 슬롯이 아니라 **상대 슬롯 중 가장 빠른 것** | 대상이 **속도로** 정해진다 → `RollSpeedDice` 뒤라야 의미가 있다 |
| UI 는 **속도 내림차순 좌→우**. 풀 순서는 고정 | 정렬은 **표시 전용**이다 |

**그래서 이렇게 된다**

- **게이트는 `BoutGraph.RegisterAction` 한 곳이다.** `action.SourceSlot` 이 봉인이면 거부.
  **타겟 경로는 무수정** — edge 가 봉인 슬롯을 가리키는 것이 정상이다.
  타겟까지 막아야 했으면 `PlayerActionInput`·`BoutGraph`·`CombatExecutor`·UI 를 전부 손봐야 했다
- **`_slotRuntimeMap` = 풀 전체가 규칙으로 확인됐다.** 봉인 슬롯이 **공격 대상이 될 수 있으므로**
  신원 조회가 반드시 된다 (2026-08-21 에 `BattleRuntime.cs:54` 를 풀로 판단한 근거가 사후에 맞았다)
- **봉인 슬롯을 때리면 항상 일방 공격이다.** 카드를 못 내니 액션이 없고, 액션이 없으면 합이 성립할 수
  없다. 빈 슬롯 때리기와 같은 경로라 **새로 짤 코드가 0**이다.
  즉 봉인의 효과는 "슬롯 하나를 잃음" + "**막을 수 없는 표적 하나를 상대에게 줌**" 두 개다
- **한 필드가 두 축을 겸하고 있었다.** 이것이 "2번만 봉인" 에서 깨진 이유다:

  | 축 | 무엇이 바꾸나 | 타입 | 사는 곳 |
  |---|---|---|---|
  | 슬롯을 **몇 개** 가졌나 | 감정 레벨 | 개수 | `_speedSlots.Count` / `SetSpeedSlotCount` |
  | 그중 **이번 턴에 쓰나** | 봉인 | 플래그 | `SpeedSlotRuntime.IsSealed` |

**표시 순서 — 두 규칙이 합쳐진다 (파생. 직접 관측한 것이 아니다)**

```
[봉인 슬롯들] → [비봉인, 속도 내림차순]
```
봉인은 속도 정렬의 **예외**다. 봉인 슬롯엔 속도가 없거나 의미가 없으므로 속도로 정렬하면
오히려 오른쪽 끝으로 가야 하는데 실제로는 맨 왼쪽이다.

- **정렬 기준은 `ActionPriority.CompareTo` 와 같아야 한다** — 속도 내림 → `CharacterId` 오름 →
  `SlotIndex` 오름. 다르면 **동점에서 화면이 실행 순서를 거짓말한다.**
  2026-08-05 에 행동 순서가 통째로 뒤집혀 있던 것이 오래 안 보였던 이유가 "화면에 실행 순서가
  안 나온다" 였다. 정렬을 같은 기준으로 맞추면 그 계열 버그가 눈에 보이게 된다
- **`SlotIndex` 를 다시 부여하면 안 된다.** "가장 빠른 슬롯이 0번" 은 **화면 위치**를 말하는 것이고
  슬롯의 신원이 아니다. `SpeedSlot` 은 `BoutGraph.edges` / `_slotRuntimeMap` / `interceptCandidates` 의
  **키**라서, 매 턴 번호를 다시 매기면 등록된 액션과 합이 통째로 미아가 된다.
  `SlotDebugItem.Bind` 가 `SpeedSlotRuntime` **객체**를 받으므로 신원은 객체가 나른다 — 표시 순서는
  마음대로 바꿔도 안전하다
- 지금 `SlotDebugPanel` 은 **양 진영을 한 목록**에 평탄하게 그린다. 원작 규칙은 캐릭터별 행 안에서의
  정렬이므로, 디버그 패널을 전체 정렬할지 캐릭터별로 정렬할지는 별개 선택이다.
  전체 정렬하면 그 목록이 곧 **bout 실행 순서**가 되어 디버그 가치가 올라간다

**봉인 슬롯이 속도를 굴리는지는 관측할 수 없다 — 그래서 규칙 질문이 아니라 코드 결정이다.**
봉인 슬롯의 `Speed` 를 읽는 코드가 하나도 없다: 액션이 없어 `ActionPriority` 에 안 들어가고,
합 판정에도 안 쓰이고, 화면엔 숫자가 안 보인다.
**안 굴리는 쪽을 권한다** — 깨진 슬롯에 속도가 없는 게 자연스럽고, 봉인 대상 선정("가장 빠른 슬롯")에서
이미 봉인된 슬롯이 후보로 되돌아오는 것도 같이 막힌다.

**아직 안 정해진 것**
- 봉인의 **발동 조건과 지속시간**. "가장 빠른 슬롯이 계속 잠긴다" 는 관측이 (a) 같은 슬롯이 계속
  잠겨 있는 것인지 (b) 매번 그때의 최속을 새로 잠그는 것인지 가르지 않는다
- 감정 레벨이 **내려가는** 경우가 있나. 있으면 "풀은 4인데 이번 턴엔 3" 이 되살아나 개수 축이
  다시 필요해진다. 없으면 `_speedSlots.Count` 가 곧 슬롯 수다

**⚠ 새로 생긴 슬롯은 그 턴에 속도가 0이다 (2026-08-21 발견).**
`BattleManager.StartTurn` 이 `RollSpeedDice()` → `TurnStartEvent`(→ `SpeedSlotPassive`) 순인데
슬롯이 느는 것은 뒤쪽이다. `SpeedSlotRuntime` 생성자는 `Speed` 를 굴리지 않으므로(`Roll()` 만 굴린다)
그 슬롯은 `Speed = 0` 인 채로 한 턴을 보낸다. **위 `_slotRuntimeMap` 크래시를 고치면 그다음에 만날 증상.**

~~**⚠ `SetSpeedSlotCount` 는 절대값 setter라 효과끼리 합성이 안 된다 (2026-08-21 발견).**~~
✔ **해소 (2026-08-22).** `_activeSpeedSlotCount` 를 지우면서 몸통이 `while` 하나만 남아
**단조(monotone)** 가 됐고, 이름도 `EnsureSpeedSlotCount` 로 따라갔다.
이제 여러 효과가 불러도 **큰 값이 이기고** 매 턴 작은 값을 다시 불러도 안 깎인다.
**줄이는 일을 아예 못 하는 것이 안전장치가 됐다.** 봉인은 개수가 아니라 `IsSealed` 플래그로 가므로
축이 겹치지도 않는다 — 걱정했던 "다음 턴 시작에 조용히 풀린다" 는 구조적으로 못 일어난다.

**주의**: `Bout.cs` 때 세운 기준("참조 0건만으로 데드 코드 판단 말 것")의 **반대 방향** 사례들이다.
그때는 지워야 할 것이 남아 있었고, 이번엔 불러야 할 것이 안 불렸다. 분류할 때 둘을 섞지 말 것.

**여덟 번째 건은 세 번째 종류다 (2026-08-05).** `Use()`/`Used` 는 안 불렸는데 **동작은 이미
맞았다** — `Reuse` 가 원하는 것("커서를 안 움직인다")이 아무것도 안 해도 달성되기 때문이다.
앞의 일곱 건은 안 부르면 증상이 났지만 이건 증상이 없다. **그래서 배선 여부가 정확성 문제가
아니라 설계 선택이 된다**(3-1.10 5단계에서 배선하는 쪽으로 결정). 증상 없는 미배선은
"안 불려도 되는 것"일 수 있으니 지울지 부를지를 먼저 정할 것.

#### 전수조사 결과 (2026-07-31 실시, `Engine/` public 멤버 308개)

**삭제 완료 (4)** — 전부 중복이거나 대체됨:
- `CharacterRuntime.IncreaseMaxHp` / `IncreaseMaxStagger` / `IncreaseMaxEnergy` —
  `ChangeMaxHp`/`ChangeMaxStagger`/`ChangeMaxEnergy` 가 따로 있고 `ChangeMax*Event` 가 그쪽을 부른다.
  같은 일을 하는 메서드가 두 벌이었다
- `BattleRuntime.HasEvents` / `BattleRuntime.GetSpeedSlotRuntime`
- `StatusEffectRuntime.ReduceStack` — Bleed/Burn 이 `Stack = Stack / 2` 로 필드를 직접 만진다.
  **반대 선택지였던 "효과들이 이걸 쓰도록 통일"은 채택하지 않았다** — 지금 스택 감소가 `AddStack` 을
  안 거치는 유일한 경로라 일관성이 깨져 있다는 점은 남아 있다

**삭제 결정 (1) — `SpeedSlotRuntime.Used` / `MarkUsed` (2026-08-06)**:
- 초판 진단은 "**부르는 곳만 없다**"였는데 **절반만 맞았다. 불러도 살아남을 시간이 없다.**
  `BattleManager.EndTurn()`(`:97~111`)이 `ExecuteCombat` → `TurnEndEvent` → `BoutGraph.Clear()`
  → `StartTurn()` → `RollSpeedDice()` 를 **한 클릭 안에서 전부 동기로** 돌고,
  `SpeedSlotRuntime.Roll()` 이 `Used = false` 를 같이 한다. 해석 도중 잠깐 `true` 였다가
  같은 클릭이 끝나기 전에 전부 꺼진다 — 플레이어가 볼 시점엔 항상 흰색이다
- **원작에 대응물이 없다 (2026-08-06 사용자 확인).** 원작은 슬롯별 "행동 끝" 표시를 하지 않는다.
  **전투 해석에 들어가는 순간 슬롯 UI 가 통째로 비활성화되고**, 그 자리에 각자 쓰는 카드와
  그 카드의 주사위 UI 가 나와서 연출된다. 즉 `Used` 는 이 프로젝트가 만들어낸 개념이다
- 따라서 **`Used` / `MarkUsed` / `Roll()` 의 `Used = false` / `SlotDebugItem` 의 회색 분기**를
  전부 지운다. 읽는 곳은 `SlotDebugItem.cs:56` 하나뿐이었다
  (`CardManager.Used` 와 `DiceState.Used` 는 이름만 같은 남남이다)
- **분류 주의**: 이건 9번의 "안 불려서 증상이 난 일곱 건" 이 아니라 3-1.10 5단계에서 나온
  **세 번째 종류(증상 없는 미배선)** 다. 그때 세운 기준 — "지울지 부를지를 먼저 정할 것" — 을
  적용한 첫 사례이고, 답이 "지운다" 로 나온 첫 사례이기도 하다

**삭제 후보 (1 신규)** — ✔ **처리됨. 2026-08-19 확인 시 이미 없었다** (`HasEvents` 와 함께 지워진 듯):
- ~~`BattleRuntime.SlotRuntimeMap`~~ — 지운 `GetSpeedSlotRuntime` 과 같은 종류였다. `_slotRuntimeMap` 은
  생성자에서 `BoutGraph` 에 넘겨져 `BoutGraph.SlotRuntime` 으로 쓰이므로 `BattleRuntime` 쪽 접근자는 잉여

**미래용 — 남김 (7)**:
- **`BattleRuntime.CombatLogs` + `LogDispatcher.DispatchAll(IReadOnlyList<CombatLog>)`** —
  **이 둘이 짝이고 리플레이 인프라의 절반이다.** 전자가 기록을 모으고 후자가 UI 에 재생한다.
  "리플레이는 시드 + 입력열, 이력은 로그"라는 결론이 우연이 아니라 이미 그 구조가 만들어져 있고
  배선만 안 된 것
- `DicePool.Inject` — 전투 중 주사위 추가용. 명시적으로 남기기로 결정
- `DiceData.Effects` — `DiceEffect` enum 이 비어 있음
- `CardResolver.BuildCardEffects` — TODO 주석 있음
- `CardManager.Discard` / `Exile` — UI 용 읽기 전용 접근자

**오탐 — 건드리면 안 됨 (1)**:
- `SpeedSlot.GetHashCode` — `override` 라 `Dictionary`/`HashSet` 이 암묵 호출한다.
  `BoutGraph.edges`, `CombatExecutor.visited` 가 전부 여기 의존

**조사 한계**: 이름 충돌은 거짓 음성을 만든다(`Clear`/`Count` 처럼 다른 타입에도 있는 이름은
"쓰이는 중"으로 집계돼 빠진다). 반대 방향은 없으니 **위 목록은 확실하고, 놓친 게 더 있을 수는 있다.**
1차 스캔은 제네릭 타입의 공백(`Dictionary<A, B>`)도 놓쳤다 — `SlotRuntimeMap` 이 2차에서야 나왔다.

### 10. UI — 나중에 만들 것 (2026-08-06)

- **전투 해석 연출 (원작 흐름, 사용자 확인 2026-08-06 — 만들 계획 있음)**
  - **해석에 들어가면 슬롯 UI 가 비활성화된다.** 그 자리에 각자 쓰는 카드와 그 카드의
    주사위 UI 가 나오고, **합을 하나씩** 보여준다. 그래서 슬롯에 "행동 끝" 표시가 없다
    (9번의 `Used` 삭제 근거)
  - **이벤트 시스템을 뜯을 필요가 없을 가능성이 높다.** 지금 해석은 `EndTurn()` 한 클릭 안에서
    DFS 로 통째로 끝나는데(7번), 연출은 **해석은 그대로 동기로 끝내고 `CombatLogs` 를
    시간차를 두고 재생**하면 된다. 그 인프라가 이미 절반 있다 —
    `BattleRuntime.CombatLogs` + `LogDispatcher.DispatchAll(IReadOnlyList<CombatLog>)`
    (9번 "미래용 — 남김" 의 첫 항목, "리플레이 인프라의 절반" 으로 적혀 있는 그것)
  - **그래서 오늘까지 한 로그 작업이 전부 이 연출의 재료다.** 3-1.11(`BaseRoll`/`ModifiedRoll`),
    `UnopposedLog` 배선, `WasClash`, `DamageLog`/`StaggerLog` 의 `AttackerId` —
    **연출이 화면에 그려야 할 것을 로그가 다 갖고 있어야 재생이 가능하다.**
    "로그가 사실을 다 담게 하기" 가 디버깅 편의가 아니라 기능의 전제였던 셈
  - **7번(`Step()` 재귀)의 우선순위가 내려간다.** DFS→BFS 전환이 연출의 전제가 아니다
  - 아직 안 정해진 것: 재생 중 **모델을 읽는 UI**(`HpUI` 등)를 어떻게 할 것인가.
    해석이 이미 끝나 있으므로 모델은 최종 상태고, 로그를 재생해도 HP 는 처음부터 최종값이다.
    `StatusUI` 처럼 "로그가 트리거, 데이터는 모델" 인 UI 가 전부 걸린다
    (`DiceUI` 가 "로그가 곧 데이터" 라 안 걸리는 것과 대비 — 그 구분이 여기서 값을 한다)

- ~~**슬롯 UI 속도순 정렬**~~ — ✔ **완료 (2026-08-22, 플레이 검증)**
  - **원작 규칙 (사용자 플레이 확인)**: 슬롯은 **속도 내림차순 좌→우**로 나열된다.
    풀 순서는 고정이고 **표시만** 정렬된다. 봉인 슬롯은 이 정렬의 **예외**로 맨 왼쪽
    (위 9번 "봉인 규칙 확정" 참고)
  - **정렬 기준은 `ActionPriority.CompareTo` 와 같아야 한다** — 속도 내림 → `CharacterId` 오름 →
    `SlotIndex` 오름. 다르면 동점에서 화면이 실행 순서를 거짓말한다.
    UI 는 오름차순 리스트라 **비교자 부호가 `ActionPriority`(max-heap) 와 반대**다.
    2026-08-05 에 두 반전이 겹쳐서 행동 순서가 통째로 뒤집혔던 그 자리다
  - **결정 (사용자, 2026-08-22): 지금은 `SlotDebugPanel` 을 전체 정렬한다.**
    원작은 캐릭터별 행 안에서의 정렬이지만 저건 디버그 패널이고, 전체 정렬하면
    **목록이 곧 bout 실행 순서**가 되어 값이 있다. 진짜 캐릭터별 UI 를 만들 때 그때 고친다
  - `CharacterId` 가 비교자에 필요한 것은 **전체 정렬이기 때문**이다. 캐릭터별로 가면
    그 줄은 상수가 되어 죽고, 대신 바깥 루프를 id 로 정렬해야 한다(딕셔너리 순회는 순서 보장이 없다).
    **어느 쪽을 골라도 `CharacterId` 는 등장하고 위치만 다르다**
  - `SlotIndex` 는 두 경우 다 필요하다. **한 캐릭터 안에서도 속도 동점이 흔하고**
    (같은 min~max 로 각자 굴린다) `List.Sort` 는 불안정 정렬이다
  - **⚠ `_speedSlots` 자체를 정렬하면 안 된다.** `EnsureSpeedSlotCount` 가
    `new SpeedSlot(id, _speedSlots.Count)` 로 **리스트 위치 = 슬롯 번호**를 전제한다.
    뒤섞으면 다음에 만들어지는 슬롯이 이미 있는 번호를 받아 `_slotRuntimeMap` 키가 충돌한다.
    **UI 쪽 복사본을 정렬한다**
  - **LINQ 금지.** 이 저장소에 `using System.Linq` 가 0건이다. `List.Sort(비교자)` 로 충분하고,
    재사용 리스트를 필드로 두면 `Refresh` 마다 할당도 없다
  - **검증 실시 완료**: `Ally01` 1/1 · `Enemy01` 9/9 로 벌려 적 슬롯이 전부 왼쪽에 오는 것 확인 →
    표준 세팅(5/5 · 1/1)으로 되돌려 순서가 따라 뒤집히는 것까지 확인.
    **데이터를 뒤집었을 때 화면도 뒤집히는가**가 판정 기준이었다 — 상수를 박아둔 경우가 배제된다
  - **구현**: `SlotDebugPanel` 에 재사용 리스트 `_sorted` + `static int CompareBySpeed(...)`.
    `ActionPriority` 를 재사용하지 않고 따로 쓴 이유는 **부호가 반대**이기 때문이다
    (저긴 max-heap, 여긴 오름차순 리스트). 같은 규칙을 두 번 적는 셈이라 한쪽만 고치는 사고가
    가능하다 — **동점 순서가 화면과 실행에서 갈리면 여기를 볼 것**
  - **⚠ 표준 검증 세팅을 건드렸으면 되돌릴 것.** `Ally01` 5/5 · `Enemy01` 1/1 위에
    기존 레시피들이 전부 서 있다("아군이 먼저 돈다"). 바꾼 채로 커밋하면 다음에 그 레시피가
    반대 순서로 돌고 증상이 엉뚱하게 보인다

- **캐릭터별 주사위 큐 표시 (원작 UI)** — 사용자 결정: 지금은 전역 로그 콘솔로 디버깅하고,
  **진짜 캐릭터별 UI 는 나중에 따로 만든다.**
  - **로그가 아니라 상태를 읽는 물건이다.** 원작에서 슬롯에 보이는 건 "지난 굴림 기록"이 아니라
    **지금 이 캐릭터에게 걸린 주사위 목록**(타입 + 범위)이다. `CombatLogs` 가 아니라
    `DicePool` 을 읽어야 하고, 따라서 `BaseRoll`/`ModifiedRoll` 은 거기 안 나온다
  - 지금 `DicePool` 을 밖에서 훑을 창구가 없다(`Peek` 은 커서 하나만 준다). 큐 전체를 보여주려면
    읽기 전용 열거가 필요한데, **`DiceEntry` 를 그대로 내보내면 UI 가 `CurrentRoll` 을 읽게 된다** —
    `GetDiceInfo` 를 `DiceInfo` 반환으로 만든 것과 같은 이유로 막아야 한다
  - 5대5 면 인스턴스가 10개다. `CardHandUI` 와 같은 문제("선택된 캐릭터 것만 보이기")를 만난다
- ~~**`DamageLog` 에 공격자가 없다**~~ — ✔ **해결 (2026-08-06).** `DamageLog` 과 `StaggerLog`
  둘 다 `AttackerId` 를 갖는다. 캐릭터별 패널을 만들 때 "내 주사위가 얼마 때렸나"를
  내 패널에 띄울 수 있다
- **전역 로그 콘솔**(`DiceUI` → `CombatLogUI`)은 검증용 계기다. `LogDispatcher` 를 6종
  구독해서 줄을 쌓는다. **`StatusUI` 와 구조가 다르다** — 거기선 로그가 트리거고 데이터는
  모델에서 오는데, 여기선 **로그가 곧 데이터**다(굴림값은 모델에 안 남는다). 그래서
  `Refresh()` 가 아니라 `Append(line)` + `Redraw()` 다

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
4계층: `CharacterData`(SO 청사진) → `CharacterModel`(순수 청사진) → `CharacterState`(스냅샷) →
`CharacterRuntime`(가변). `CharacterModel` 은 2026-08-20 에 생겼다 — Engine 이 SO 를 안 보게 하는 관문.

**목표: Unity 에 최대한 의존하지 않는 자체 전투 엔진.** — ✔ **달성 (2026-08-21).**
`Engine/` 은 `LOR.Engine.asmdef`(`noEngineReferences: true`)로 갈린 별도 어셈블리라
**`using UnityEngine` 도 SO 타입도 컴파일러가 막는다.** `DeterministicRng` 까지 Engine 안에 있어
`LOR.Engine.dll` 은 Unity 없이 콘솔에서 돈다(아직 실제로 돌려보진 않았다). 자세한 것은 "다음 작업 4".

**단 "순수" 는 의존성 의미다. 부작용이 없다는 뜻이 아니다** — `CharacterRuntime` 은 가변이고
`Event.Apply` 가 상태를 바꾼다. 순수 계산은 `CombatExecutor` 한 곳으로 격리돼 있다.

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

폴더가 의존 방향을 나타낸다. **2026-08-21 부터는 `Engine/` 이 asmdef 로 갈린 별도 어셈블리라
그 방향을 컴파일러가 강제한다** — `Engine` → `Data`/`Scene`/`UI` 는 순환 참조가 되어 불가능하다.
나머지 셋은 `Assembly-CSharp` 에 함께 남아 있고 그들 사이에는 경계가 없다.

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
