using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class CharacterRuntime : IEventSink
{
    private readonly CharacterState _state;
    private readonly IEventSink _eventSink;
    private readonly DicePool _dicePool = new();
    private readonly Dictionary<int, DiceEntry> _diceById = new();
    private int _nextDiceId = 0;
    private int _currentHp;
    private int _currentStagger;
    private readonly List<StatusEffectRuntime> _statusEffects = new();
    private readonly Dictionary<StatusEffectType, StatusEffectRuntime> _effectMap = new();
    private readonly List<SpeedSlotRuntime> _speedSlots = new();
    private readonly List<PassiveEffect> _passives = new();
    private bool _dirty;
    public bool IsDead => _currentHp <= 0;
    public bool IsStaggered => _currentStagger <= 0;
    private int _emotionLevel;
    private int _emotionStack;
    private int _activeSpeedSlotCount;

    public int CharacterId => _state.CharacterId;
    public int BaseSpeedSlotCount => _state.SpeedSlotCount;
    public int EmotionLevel => _emotionLevel;   
    public int EmotionStack => _emotionStack;
    public IReadOnlyList<SpeedSlotRuntime> SpeedSlots => _speedSlots;

    public CharacterRuntime(CharacterState state, IEventSink eventSink)
    {
        _state = state;
        _eventSink = eventSink;
        _currentHp = state.MaxHp;
        _currentStagger = state.MaxStagger;
        _activeSpeedSlotCount = state.SpeedSlotCount; // 초기값
        CreateSpeedSlots();

        // 패시브 효과 초기화
        foreach (var passiveData in state.Passives)
        {
            var passive = PassiveFactory.Create(passiveData, this);
            _passives.Add(passive);
        }
    }

    public void EnqueueEvent(ICombatEvent ev) => _eventSink.EnqueueEvent(ev);

    public void TakeDamage(int amount)
    {
        _currentHp -= amount;
        if (_currentHp < 0)
            _currentHp = 0;
    }
    public void TakeStagger(int amount)
    {
        _currentStagger -= amount;
        if (_currentStagger < 0)
            _currentStagger = 0;
    }
    public void RecoverStagger(int amount)
    {
        _currentStagger += amount;
        if (_currentStagger > _state.MaxStagger)
            _currentStagger = _state.MaxStagger;
    }
    public void GainEmotionStack(int stack) // 나중에 긍정 감정 & 부정 감정 섞이면 로직 고쳐야 함
    {
        _emotionStack += stack;

        if (_emotionStack >= _state.MaxEmotionStack)
        _emotionStack = _state.MaxEmotionStack;
    }
    public void SetSpeedSlotCount(int count)
    {
        while (_speedSlots.Count < count)
        {
            var slot = new SpeedSlot(_state.CharacterId, _speedSlots.Count);
            _speedSlots.Add(new SpeedSlotRuntime(slot, _state.MinSpeed, _state.MaxSpeed));
        }
        _activeSpeedSlotCount = count;
    }

    // �ֻ���
    public void UseAction(ActionInstance action)
    {
        foreach (var diceData in action.Card.dices)
        {
            int id = _nextDiceId++;
            var handle = new DiceHandle(new CharacterHandle(_state.CharacterId), id);
            var runtime = new DiceRuntime(diceData, action);
            _diceById[id] = new DiceEntry(runtime, handle);
            _dicePool.Inject(new DiceEntry(runtime, handle));
        }
    }

    public DiceEntry? Peek() => _dicePool.Peek();
    public void Advance(AdvanceType type) => _dicePool.Advance(type);
    public void RecoverDice() => _dicePool.Recover();
    public void ResetDiceForNextTurn() => _dicePool.ResetForNextTurn();

    // �����̻�
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
        // 
        var ctx = new TurnStartContext(this);
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnTurnStart(ctx);
        FlushExpired();

        //패시브 효과 트리거
        foreach (var passive in _passives)
            passive.OnTurnStart(ctx);
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
    public void TriggerBeforeClash(ClashContext ctx, bool isOwnerA)
    {
        // 상태이상
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnBeforeClash(ctx, isOwnerA);
        FlushExpired();

        // 패시브
        foreach (var passive in _passives)
            passive.OnBeforeClash(ctx, isOwnerA);
    }

    public void TriggerDiceClash()
    {
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnDiceClash();
        FlushExpired();
    }

    public void TriggerTurnEnd()
    {
        foreach (var effect in _statusEffects)
            effect.OnTurnEnd();
        FlushExpired();

        if (_emotionStack == _state.MaxEmotionStack)
        {
            EnqueueEvent(); //감정 레벨 올리는 이벤트 만들기
        }
    }
    public void TriggerBeforeStagger(StaggerContext ctx)
    {
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnBeforeStagger(ctx);
        FlushExpired();
    }

    public void TriggerAfterStagger(StaggerContext ctx)
    {
        EnsureSorted();
        foreach (var effect in _statusEffects)
            effect.OnAfterStagger(ctx);
        FlushExpired();
    }

    void CreateSpeedSlots()
    {
        for (int i = 0; i < _state.SpeedSlotCount; i++)
        {
            var slot = new SpeedSlot(_state.CharacterId, i);
            _speedSlots.Add(new SpeedSlotRuntime(slot, _state.MinSpeed, _state.MaxSpeed));
        }
    }

    void EnsureSorted()
    {
        if (!_dirty) return;
        _statusEffects.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        _dirty = false;
    }

    void FlushExpired()
    {
        for (int i = _statusEffects.Count - 1; i >= 0; i--)
        {
            if (_statusEffects[i].IsExpired)
            {
                _effectMap.Remove(_effectMap.FirstOrDefault(x => x.Value == _statusEffects[i]).Key);
                _statusEffects.RemoveAt(i);
            }
        }
    }
}