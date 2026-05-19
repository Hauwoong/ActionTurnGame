# CLAUDE.md — Library of Ruina 리팩토링 프로젝트

## 프로젝트 개요
Unity / C# 카드 + 주사위 전투 시스템 (LOR 스타일).
3계층: `Character`(불변) → `CharacterState`(스냅샷) → `CharacterRuntime`(가변).
이벤트 큐 기반 (`BattleRuntime._eventQueue` → `Step()` 즉시 실행).

## 현재 상태 (중요)
**프로젝트가 빌드되지 않을 가능성이 높음.** 5단계 Character 리팩토링이 도중에 멈춰서 `Player.cs`/`Enemy.cs` 가 사라진 메서드를 `override` 하고 있음. 게다가 **상속 모델 자체를 폐기**할 예정이므로 단순 수정이 아니라 재작성. 작업 시 항상 컴파일 가능 여부부터 의심.

## 목표 구조 (변경됨)
**`Player : Character` 상속 폐기. 소유 관계로 전환.**

```
Player  (영구 / DontDestroyOnLoad)
  └── List<Character> Roster        // 보유한 모든 캐릭터
  └── List<Character> SelectedParty // 이번 전투 출전 멤버

Enemy   (조우 단위)
  └── List<Character> Members

Character (MonoBehaviour, 전투 씬 한 명당 하나)
  ├── [SerializeField] CharacterData _data        // 청사진 (영구)
  └── CharacterRuntime Runtime { get; private set; } // 전투 동안만, BattleManager 가 BindRuntime() 주입
```

- 덱빌딩 / 캐릭터 정보 UI 는 `Character.CharacterData` 만 읽어서 표시.
- 전투 시작: `BattleManager.StartBattle(player.SelectedParty, enemy.Members)` → 각 Character 의 Data 로 Runtime 빌드 후 `BindRuntime`.
- 전투 중 상태(HP/Energy/Dice/Status/Emotion)는 전부 `Character.Runtime` 에서 읽고 변경.
- `Player.cs`/`Enemy.cs` 는 **전투 캐릭터가 아님.** `currentHP`, `TakeDamage`, `Die` 같은 메서드 없음.

## 작업 우선순위 (REFACTORING_DIAGNOSIS.md 기반)

### P0 — 구조 재작성 + 컴파일 살리기
1. **`Charater/Character.cs`** : 한 명의 전투 캐릭터로 재정의.
   - `[SerializeField] CharacterData _data` + `CharacterRuntime Runtime { get; private set; }` + `BindRuntime(CharacterRuntime)`.
   - 옛 `currentHP/MaxHP/TakeDamage/Die/OnTurnStart` 메서드 부활 금지. 모두 Runtime 위임.
2. **`Charater/Player.cs`** : 통째로 재작성.
   - `: Character` 상속 제거.
   - `List<Character> Roster`, `List<Character> SelectedParty` 또는 동등 구조.
   - 덱·UI 이벤트(`OnHPChanged` 등)·`battlemanager.EndBattle()` 직접 호출 제거.
3. **`Charater/Enemy.cs`** : 통째로 재작성. Player 와 같은 패턴 (`: Character` 제거, 멤버 명부 보유).
4. `BattleManager/CombatEngine/LorRuleSet.cs:34` — `BuildEvents` 에 `return events;` 없음.
5. `UI/PlayerActionInput.cs:9,16,27` — `SpeedSlot` 은 struct인데 `= null`, `.index` 사용. → `SpeedSlot?` + `.SlotIndex`.
6. `Bout/BoutResolver.cs:4,26` — 파일명/클래스명 불일치(`BoutPlanner`), 정의 없는 `UpdateRealationsFor` 호출.

### P1 — 데드 코드 / 오타
- 삭제 대상(자기 파일에서만 참조): `Dice/DiceQueue.cs`, `BattleManager/Runtime/ActionRuntime.cs`, `CombatLog/EventClass/HpChangeEvent.cs`.
- 오타 일괄 변경 `Destory → Destroy` (`ActionRuntime.cs:17`, `DiceQueue.cs:37`, `CombatLog.cs:7` enum 값, `LorRuleSet.cs:21,22`). enum 값 변경은 `[FormerlySerializedAs]` 검토.
- 폴더명 `Charater/ → Character/`.
- `StatusDamageEvent.cs:22` `_ctx.Defender.Die();` 삭제 (DeathEvent 와 중복).

### P2 — 구조 정합성
- `Context/ClashContext.cs` → `: IClashContext` 추가.
- `Passive/PassiveFactory.cs` — `EmotionOnAttack`, `MaxHpBoost` 미등록. Stat-modifier 와 Runtime-effect 분리: `IStatModifierPassive` 는 CharacterStateBuilder 에서, `PassiveEffect` 만 Factory.
- `Player.hand` / `Enemy.hand` `public List<>` → `IReadOnlyList<>` 캡슐화.
- `EmotionLevelUpEvent.cs:4` 필드 → `{ get; }`.
- Player.cs / Enemy.cs UTF-8 재저장 (한글 주석 CP949 깨짐).

## 핵심 규칙
- **상태 변경은 Event.Apply 안에서만.** `runtime.AddLog → CharacterRuntime 호출 → 후속 Event Enqueue` 순.
- **CombatExecutor = 순수 계산.** 부작용 금지.
- **CardData 등 Data 계층은 Runtime 모르게.** 현재 `CardData` 가 `BattleContext` 의존 — 신규 코드에선 금지, 기존은 `CardResolver` 서비스로 분리 예정.
- **UI 는 LogDispatcher 구독.** 새 UI 가 `Player.OnHPChanged` 류 옛 이벤트에 묶이지 않게.
- **Deterministic.** RNG 는 `IRng` 통해서만, Seed 기반.
- **데드 코드 격리 후 삭제.** 남길 거면 `_legacy/` 폴더로.

## 네이밍 / 스타일
- 파일명 = 클래스명. (BoutResolver 케이스 재발 금지)
- public 필드 금지, `{ get; private set; }` 또는 `IReadOnlyList<>`.
- Event/Log 1:1 매칭 유지. 새 Log 추가 시 대응 Event 도 같이.
- 한글 주석 OK, 단 UTF-8.

## 참고 문서
- `REFACTORING_DIAGNOSIS.md` — 라인 단위 진단 리포트.
- DEVLOG (`.project-cache/.../DEVLOG_Summary.txt`) — 1~6단계 설계 의도.
