# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 작업 방식 (최우선 — 다른 지시보다 우선함)

**Claude 는 코드를 작성하지 않는다.** 구현은 전부 사용자가 직접 한다.
사용자가 실력을 기르는 것이 이 프로젝트의 목적 중 하나이므로, 대신 짜주는 것은 그 목적을 해친다.

Claude 가 하는 일:
- 코드 읽기, 원인 분석, 진단, 영향 범위 조사
- 설계 선택지 제시와 트레이드오프 설명 (권장안 포함)
- 사용자가 작성한 코드에 대한 리뷰와 피드백
- 어디를 어떻게 고쳐야 하는지 **말로** 설명. 방향 제시용 짧은 코드 조각(시그니처, 한두 줄)은 허용
- 커밋 메시지 작성, git 작업, 문서 갱신

Claude 가 하지 않는 일:
- `Write` / `Edit` 로 `.cs` 파일 생성·수정
- "제가 구현할까요?" 제안
- 파일 통째 재작성 코드 블록 제공

사용자가 **명시적으로** 요청할 때만 예외. 애매하면 물어볼 것.

## 프로젝트 개요
Unity / C# 카드 + 주사위 전투 시스템 (LOR 스타일).
3계층: `CharacterData`(청사진) → `CharacterState`(스냅샷) → `CharacterRuntime`(가변).

## 현재 상태 (2026-07-21 기준)

**컴파일 통과.** 전투 시작 → 턴 루프 → 데미지까지 코드상 연결 완료.
**단 아직 한 번도 실행해보지 않았다.** 씬 세팅 후 플레이 검증이 다음 순서.

씬 세팅 필요:
1. 빈 GameObject 에 `BattleStarter` 부착
2. `battleManager` / `player` / `enemy` 인스펙터 연결
3. `Player.SelectedParty`, `Enemy.Members` 에 `Character` 할당
   — **비어 있으면 첫 턴에 즉시 전투 종료됨** (한쪽 진영 생존자 0 = 전멸 판정)

## 구조

```
Player  (현재는 씬 오브젝트. DontDestroyOnLoad 는 미구현)
  └── IReadOnlyList<Character> Roster / SelectedParty

Enemy   (조우 단위)
  └── IReadOnlyList<Character> Members

Character (MonoBehaviour, 전투 씬 한 명당 하나)
  ├── [SerializeField] CharacterData _characterData
  └── CharacterRuntime Runtime { get; private set; }   // BindRuntime() 주입
```

전투 흐름:
```
BattleStarter.Start
  → BattleManager.CreateBattle(SelectedParty, Members)   // 두 리스트 = 진영 구분
      → BattleSnapShot → CharacterState(Team 부여) → CharacterRuntime
      → OnBattleCreated → UI 바인딩
  → StartTurn                    // 속도 주사위 + TurnStartEvent
      → PlayerActionInput 로 슬롯에 카드 등록 → BoutGraph
      → TurnUI 턴 종료 버튼 → BattleManager.EndTurn
          → ExecuteCombat        // BoutStartEvent → 클래시 → DamageEvent
          → TurnEndEvent, BoutGraph.Clear
          → TryGetBattleResult ? EndBattle : StartTurn
```

- 진영은 `enum Team { Ally, Enemy }`. `CharacterState` 가 보유, `CharacterRuntime` 이 위임.
- `BattleRuntime.TryGetBattleResult(out Team? winner)` — 한쪽 전멸 시 true. 양측 동시 전멸이면 `winner == null`(무승부).
- 룰 엔진은 `DiceRuleTable`(4×4 주사위 타입 매트릭스) + `CombatExecutor`. **`IRuleSet` 계열은 폐기됨.**
- 클래시 결과는 `ClashContextEvent` 가 `DamageContext`/`StaggerContext` 로 분기.

## 남은 작업

### 1. 실행 검증 (최우선)
씬 세팅 후 플레이. 처음 밟는 경로라 `CardManager` 초기 덱, `SpeedSlotRuntime`,
`CharacterRuntime` 생성자 근처에서 예외 가능성 있음.

### 2. `Step()` 재귀 구조
`BattleRuntime.EnqueueEvent` 가 enqueue 직후 `Step()` 을 호출한다.
`Step()` 은 다른 곳에서 호출되지 않으므로 **큐에 원소가 2개 이상 쌓이지 않는다** —
사실상 `ev.Apply(this)` 와 등가이고, 이벤트는 깊이 우선(DFS)으로 즉시 처리된다.

- 의도(파생 이벤트가 전부 처리됨)는 달성되나, 메커니즘은 큐 배수가 아니라 재귀.
- `HasEvents` 프로퍼티는 아무도 쓰지 않고 외부에서 항상 `false` — 원래 배수 루프를 염두에 뒀던 흔적.
- 부작용: 전투 종료 후에도 남은 `DeathEvent` 가 처리되면 `BattleEndLog` 가 중복될 수 있음.
- 고칠 경우 DFS → BFS 로 실행 순서가 바뀌므로 전투 로그 비교 검증 필요.

### 3. 호출자 없는 잔재
`BattleInput.cs`, `BattleResult.cs`, `BattleRuntime.Start(BattleInput)`.
`Start` 의 내용은 `BattleManager.ExecuteCombat` 과 중복. 삭제 후보.

### 4. 인코딩
`DiceRuleTable.cs`, `BoutGraph.cs` 의 한글 주석이 CP949 로 깨져 있음. UTF-8 재저장 필요.

## 핵심 규칙
- **상태 변경은 Event.Apply 안에서만.** `runtime.AddLog → CharacterRuntime 호출 → 후속 Event Enqueue` 순.
- **CombatExecutor = 순수 계산.** 부작용 금지.
- **Data 계층은 Runtime 모르게.** `CardData` 가 `BattleContext` 에 의존 중 — 신규 코드 금지, 기존은 `CardResolver` 로 분리 예정.
- **UI 는 LogDispatcher 구독.** `OnBattleCreated` 로 바인딩, `OnBattleEnded` 로 해제. (`SlotDebugPanel` 이 참고 예시)
- **Deterministic.** RNG 는 `IRng` 통해서만, Seed 기반.
- **데드 코드 격리 후 삭제.** 남길 거면 `_legacy/` 폴더로.

## 네이밍 / 스타일
- 파일명 = 클래스명.
- public 필드 금지, `{ get; private set; }` 또는 `IReadOnlyList<>`.
- Event/Log 1:1 매칭 유지. 새 Log 추가 시 대응 Event 도 같이.
- 한글 주석 OK, 단 UTF-8.

## 데드 코드 판단 시 주의

**"참조 0건"만으로 판단하지 말 것. 컴파일되는지 반드시 확인.**

2026-07-21 에 `Bout.cs` 를 "참조는 없지만 개념이 살아있다"는 이유로 남겼는데,
그 파일은 `ActionInstance.Speed`(존재한 적 없는 멤버)를 읽고 있어 `721b912` 부터
빌드를 깨뜨리고 있었다. 컴파일 실패 상태에서는 Unity 가 **컴포넌트 부착을 거부**하므로,
"컴포넌트가 안 붙는다"는 증상이 나오면 먼저 컴파일 에러를 의심할 것.

에러 확인 경로 (Unity 콘솔을 못 볼 때):
```
C:\Users\a0103\AppData\Local\Unity\Editor\Editor.log
```
`grep "error CS"` — 단 이 로그는 누적되므로 **삭제된 파일의 과거 에러도 섞여 있다.**
반드시 해당 파일·멤버가 현재도 존재하는지 교차 확인할 것.

## 참고 문서
- DEVLOG (`.project-cache/.../DEVLOG_Summary.txt`) — 1~6단계 설계 의도.
- ~~`REFACTORING_DIAGNOSIS.md`~~ — 저장소에 존재하지 않음. 여기 내용이 이를 대체.
