using System.Collections.Generic;
// 전투의 가변 심장이자 이벤트 허브. 이벤트는 Apply(this)로 이 객체로 되불러 상태를 바꾼다.
public class BattleRuntime : IEventSink, ISlotLookup
{
    // ──────────── 결정성(RNG) ────────────
    private readonly int _seed;
    public IRng Rng { get; }

    // ──────────── 캐릭터 · 슬롯 ────────────
    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;

    // ──────────── 전투 · 합(Bout) ────────────
    public CombatExecutor Executor { get; private set; }

    private BoutGraph _boutGraph;
    public BoutGraph BoutGraph => _boutGraph;

    private int _nextActionId = 0;

    /// <summary>
    /// 액션 고유 번호를 하나 발급한다(발급 후 증가).  ActionInstance 식별용
    /// </summary>
    public int NextActionId() => _nextActionId++;

    // ──────────── 이벤트 ────────────
    private readonly Queue<ICombatEvent> _eventQueue = new();

    // ──────────── 로그 ────────────
    private readonly List<CombatLog> _combatLogs = new();
    public IReadOnlyList<CombatLog> CombatLogs => _combatLogs;

    private readonly LogDispatcher _logDispatcher = new();
    public LogDispatcher LogDispatcher => _logDispatcher;

    // ──────────── 생성자 ────────────
    /// <summary>
    /// 스냅샷으로 전투 런타임을 조립한다.
    /// Rng/Executor 생성 -> 캐릭터별 CharacterRuntime 생성 -> 전 캐릭터 속도 슬롯을 평탄화해 조회 맵에 등록 -> 빈 BoutGraph 준비
    /// </summary>
    /// <param name="snapShot">전투 하나를 환전히 결정하는 불변 입력(Seed + CharacterState)</param>
    public BattleRuntime(BattleSnapShot snapShot)
    {
        _seed = snapShot.Seed;
        Rng = new DeterministicRng(_seed);
        Executor = new CombatExecutor(Rng, this);
        _characters = new Dictionary<int, CharacterRuntime>();
        foreach (var state in snapShot.CharacterStates)
        {
            var runtime = new CharacterRuntime(state, this, Rng);
            _characters[state.CharacterId] = runtime;
        }
        _boutGraph = new BoutGraph(new Dictionary<SpeedSlot, ActionInstance>(), this);
    }

    // ──────────── 캐릭터 · 슬롯 ────────────
    /// <summary>
    /// 전 캐릭터의 모든 속도 슬롯을 다시 굴린다. 턴 시작 때 BattleManager가 호출
    /// </summary>
    public void RollSpeedDice()
    {
        foreach (var character in _characters.Values)
            foreach (var slot in character.SpeedSlotPool)
                slot.Roll(Rng);
    }

    /// <summary>
    /// 캐릭터 번호롤 가변 런타임을 가져온다. 등록되지 않은 번호만 예외.
    /// </summary>
    /// <param name="characterId">찾을 캐릭터 번호</param>
    /// <returns>해당 캐릭터의 CharacterRuntime</returns>
    public CharacterRuntime GetCharacterRuntime(int characterId)
        => _characters[characterId];

    public SpeedSlotRuntime GetSlotRuntime(SpeedSlot slot)
        => GetCharacterRuntime(slot.CharacterId).SpeedSlotPool[slot.SlotIndex];

    // ──────────── 주사위 ────────────
    /// <summary>
    /// 해당 캐릭터가 지금 쓸 주사위를 소비 없이 들여다본다. 없으면 null.
    /// 소비는 안 하지만 이미 죽은(Destroyed/Consumed) 주사위는 건너뛰며 커서를 밀기는 한다.
    /// </summary>
    /// <param name="characterId">대상 캐릭터</param>
    /// <returns>쓸 수 있는 주사위, 없으면 null</returns>
    public DiceEntry? PeekDice(int characterId)
        => _characters[characterId].Peek();

    /// <summary>
    /// 현재 커서의 주사위를 처리하고 다음 주사위로 넘어간다.
    /// Consume/Destroy는 커서 전진, Reuse는 커서를 두고 같은 주사위를 다시 쓴다(연격).
    /// </summary>
    /// <param name="characterId">대상 캐릭터</param>
    /// <param name="type">처리 방식(Consume/Destroy/Reuse)</param>
    public void AdvanceDice(int characterId, AdvanceType type)
        => _characters[characterId].Advance(type);

    /// <summary>
    /// 액션을 시전자에게 넘겨 실제로 사용시킨다 - 코스트 지불 이벤트 + 카드 주사위를 그 캐릭터 풀에 적재.
    /// 이 단계가 빠지면 주사위가 안 실려 데미지가 0이 된다.
    /// </summary>
    /// <param name="action">사용할 액션. 시전자는 action.SourceSlot.CharacterId로 결정된다.</param>
    public void UseAction(ActionInstance action)
        => _characters[action.SourceSlot.CharacterId].UseAction(action);

    // ──────────── 이벤트 ────────────
    /// <summary>
    /// 이벤트를 큐에 넣고 곧바로 Step()으로 처리한다.
    /// 주의: 큐지만 배수 루프가 아니다 - 파생 이벤트가 DFS로 즉시 처리되며 큐에 2개 이상 쌓이지 않는다.
    /// </summary>
    /// <param name="ev">실행할 이벤트</param>
    public void EnqueueEvent(ICombatEvent ev) // 주의: 큐지만 배수 루프가 아니다. Enqueue 직후 Step()이 즉시 실행 -> 파생 이벤트는 DFS로 즉시 처리. 큐에 2개 이상 안 쌓인다.
    {
        _eventQueue.Enqueue(ev);
        Step();
    }

    /// <summary>
    /// 큐에서 이벤트 하나를 꺼내 Apply(this)로 실행한다. 현재 호출자는 EnqueueEvent 뿐.
    /// </summary>
    public void Step()
    {
        if (_eventQueue.Count == 0) return;
        var ev = _eventQueue.Dequeue();
        ev.Apply(this);
    }

    // ──────────── 로그 ────────────
    /// <summary>
    /// 로그를 기록하고 LogDispatcher로 구독자(주로 UI)에게 전달한다.
    /// 동기 호출이라 이 줄에서 UI 콜백까지 실행된다 - 반드시 상태 변경을 먼저 하고 호출할 것.
    /// </summary>
    /// <param name="log">기록할 전투 로그</param>
    public void AddLog(CombatLog log)
    {
        _combatLogs.Add(log);
        _logDispatcher.Dispatch(log);
    }

    // ──────────── 전투 판정 ────────────
    /// <summary>
    /// 전투 종료 여부를 판정한다. 한쪽 진영이 전멸하면 true.
    /// </summary>
    /// <param name="winner">승리 팀. 양측 동시 전멸이면 null(무승부). 전투가 안 끝났으면 역시 null.</param>
    /// <returns>전투가 끝났으면 true</returns>
    public bool TryGetBattleResult(out Team? winner)
    {
        bool allyAlive = false;
        bool enemyAlive = false;

        foreach (var character in _characters.Values)
        {
            if (character.IsDead) continue;

            if (character.Team == Team.Ally) allyAlive = true;
            else enemyAlive = true;
        }

        if (allyAlive && enemyAlive)
        {
            winner = null;
            return false;
        }

        if (allyAlive) winner = Team.Ally;
        else if (enemyAlive) winner = Team.Enemy;
        else winner = null;

        return true;
    }
}
