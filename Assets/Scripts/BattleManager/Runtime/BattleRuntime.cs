using System.Collections.Generic;

public class BattleRuntime
{
    private readonly int Seed;

    private readonly List<CombatLog> _combatLogs = new();
    public IReadOnlyList<CombatLog> CombatLogs => _combatLogs;

    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;

    public IRng Rng { get; }

    private readonly Queue<ICombatEvent> eventQueue = new();
    public bool HasEvents => eventQueue.Count > 0;

    private readonly IRuleSet rules;

    public CombatExecutor Executor { get; private set; }

    private Dictionary<SpeedSlot, SpeedSlotRuntime> slotRuntimeMap = new();
    public Dictionary<SpeedSlot, SpeedSlotRuntime> SlotRuntimeMap => slotRuntimeMap;
    public BattleRuntime(BattleSnapShot snapShot)
    {
        Seed = snapShot.Seed;

        Rng = new DeterministicRng(Seed);

        rules = new LorRuleSet();

        Executor = new CombatExecutor(rules, Rng, this);

        _characters = new Dictionary<int, CharacterRuntime>();

        foreach (var state in snapShot.CharacterStates)
        {
            var runtime = new CharacterRuntime(state);

            _characters[state.CharacterId] = runtime;

            foreach (var slot in runtime.SpeedSlots)  // 맞는지 확인 부탁
            {
                slotRuntimeMap[slot.Slot] = slot;
            }
        }
    }

    public void RollSpeedDice()
    {
        foreach (var character in _characters.Values)
        {
            foreach (var slot in character.SpeedSlots)
            {
                slot.Roll(Rng);
            }
        }
    }

    public SpeedSlotRuntime GetSpeedSlotRuntime(SpeedSlot slot)
    {
        return slotRuntimeMap[slot];
    }

    public CharacterRuntime GetCharacterRuntime(int characterId)
    {
        return _characters[characterId];
    }

    public DiceRuntime GetDice(DiceHandle handle)
    {
        if (!_characters.TryGetValue(handle.Owner.CharacterId, out CharacterRuntime character)) return null;

        return character.GetDice(handle.DiceId);
    }

    public void EnqueueEvent(ICombatEvent ev)
    {
        eventQueue.Enqueue(ev);
    }

    public void Step()
    {
        if (eventQueue.Count == 0) return;

        var ev = eventQueue.Dequeue();
        ev.Apply(this);
    }

    public void AddLog(CombatLog log)
    {
        _combatLogs.Add(log);
    }
}
