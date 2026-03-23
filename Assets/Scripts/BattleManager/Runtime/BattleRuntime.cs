using System.Collections.Generic;

public class BattleRuntime : IEventSink
{
    private readonly int _seed;

    private readonly List<CombatLog> _combatLogs = new();
    public IReadOnlyList<CombatLog> CombatLogs => _combatLogs;

    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;
    public IRng Rng { get; }

    private readonly Queue<ICombatEvent> _eventQueue = new();
    public bool HasEvents => _eventQueue.Count > 0;
    public CombatExecutor Executor { get; private set; }

    private readonly Dictionary<SpeedSlot, SpeedSlotRuntime> _slotRuntimeMap = new();
    public IReadOnlyDictionary<SpeedSlot, SpeedSlotRuntime> SlotRuntimeMap => _slotRuntimeMap;

    private BattleInput _input;

    public BattleRuntime(BattleSnapShot snapShot)
    {
        _seed = snapShot.Seed;
        Rng = new DeterministicRng(_seed);
        Executor = new CombatExecutor(Rng, this);
        _characters = new Dictionary<int, CharacterRuntime>();
        foreach (var state in snapShot.CharacterStates)
        {
            var runtime = new CharacterRuntime(state, this);
            _characters[state.CharacterId] = runtime;
            foreach (var slot in runtime.SpeedSlots)
                _slotRuntimeMap[slot.Slot] = slot;
        }
    }

    public void RollSpeedDice()
    {
        foreach (var character in _characters.Values)
            foreach (var slot in character.SpeedSlots)
                slot.Roll(Rng);
    }

    // 캐릭터
    public CharacterRuntime GetCharacterRuntime(int characterId)
        => _characters[characterId];

    public SpeedSlotRuntime GetSpeedSlotRuntime(SpeedSlot slot)
        => _slotRuntimeMap[slot];

    // 주사위
    public DiceEntry? PeekDice(int characterId)
        => _characters[characterId].Peek();

    public void AdvanceDice(int characterId, AdvanceType type)
        => _characters[characterId].Advance(type);

    public void UseAction(ActionInstance action)
        => _characters[action.SourceSlot.CharacterId].UseAction(action);

    // 이벤트
    public void EnqueueEvent(ICombatEvent ev)
        => _eventQueue.Enqueue(ev);

    public void Step()
    {
        if (_eventQueue.Count == 0) return;
        var ev = _eventQueue.Dequeue();
        ev.Apply(this);
    }

    // 로그
    public void AddLog(CombatLog log)
        => _combatLogs.Add(log);

    public void Start(BattleInput input)
    {
        _input = input;
        Executor.Execute(_input.BoutGraph);
    }
}
