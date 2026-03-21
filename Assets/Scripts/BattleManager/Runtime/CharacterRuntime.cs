using System.Collections.Generic;

public class CharacterRuntime
{
    private readonly CharacterState _state;
    private readonly DicePool _dicePool = new();
    private readonly Dictionary<int, (DiceRuntime Runtime, DiceHandle Handle)> _diceById = new();
    private int _nextDiceId = 0;
    private int _currentHp;
    private readonly List<StatusEffectRuntime> _statusEffects = new();
    private readonly Dictionary<StatusEffectType, StatusEffectRuntime> _effectMap = new();
    private readonly List<SpeedSlotRuntime> _speedSlots = new();
    private bool _dirty;

    public IReadOnlyList<SpeedSlotRuntime> SpeedSlots => _speedSlots;

    public CharacterRuntime(CharacterState state)
    {
        _state = state;
        _currentHp = state.MaxHp;
        CreateSpeedSlots();
    }

    void CreateSpeedSlots()
    {
        for (int i = 0; i < _state.SpeedSlotCount; i++)
        {
            var slot = new SpeedSlot(_state.CharacterId, i);
            _speedSlots.Add(new SpeedSlotRuntime(slot, _state.MinSpeed, _state.MaxSpeed));
        }
    }

    // 주사위
    public void UseAction(ActionInstance action)
    {
        foreach (var diceData in action.Card.dices)
        {
            int id = _nextDiceId++;
            var handle = new DiceHandle(new CharacterHandle(_state.CharacterId), id);
            var runtime = new DiceRuntime(diceData, action);
            _diceById[id] = (runtime, handle);
            _dicePool.Inject(runtime);
        }
    }

    public DiceRuntime GetDice(int id)
    {
        _diceById.TryGetValue(id, out var entry);
        return entry.Runtime;
    }

    public DiceHandle GetDiceHandle(int id)
    {
        _diceById.TryGetValue(id, out var entry);
        return entry.Handle;
    }

    public DiceRuntime Peek() => _dicePool.Peek();
    public void Advance(AdvanceType type) => _dicePool.Advance(type);
    public void RecoverDice() => _dicePool.Recover();
    public void ResetDiceForNextTurn() => _dicePool.ResetForNextTurn();

    // 상태이상
    public void AddStatus(StatusEffectType type, int stack)
    {
        if (_effectMap.TryGetValue(type, out var effect))
        {
            effect.AddStack(stack);
        }
        else
        {
            var newEffect = StatusFactory.Create(type, this, stack);
            _statusEffects.Add(newEffect);
            _effectMap[type] = newEffect;
        }
        _dirty = true;
    }

    public void TriggerTurnStart()
    {
        var ctx = new TurnStartContext(this);
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnTurnStart(ctx);
        FlushExpired();
    }

    public void TriggerBeforeDamage(DamageContext ctx)
    {
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnBeforeDamage(ctx);
        FlushExpired();
    }

    public void TriggerAfterDamage(DamageContext ctx)
    {
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnAfterDamage(ctx);
        FlushExpired();
    }

    void EnsureSorted()
    {
        if (!_dirty) return;
        _statusEffects.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _dirty = false;
    }

    void FlushExpired()
    {
        _statusEffects.RemoveAll(e => e.IsExpired);
    }
}
