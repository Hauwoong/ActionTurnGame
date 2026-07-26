using System.Collections.Generic;

public class CharacterRuntime : IEventSink
{
    private readonly CharacterState _state;
    private readonly IEventSink _eventSink;
    private readonly DicePool _dicePool = new();
    private readonly Dictionary<int, DiceEntry> _diceById = new();
    private readonly CardManager _cardManager;
    private int _nextDiceId = 0;
    private int _currentEnergy;
    private int _maxEnergy;
    private int _currentHp;
    private int _maxHp;
    private int _currentStagger;
    private int _maxStagger;
    private readonly List<StatusEffectRuntime> _statusEffects = new();
    private readonly Dictionary<StatusEffectType, StatusEffectRuntime> _effectMap = new();
    private readonly List<SpeedSlotRuntime> _speedSlots = new();
    private readonly List<PassiveEffect> _passives = new();
    private bool _dirty;
    private bool _isDead = false;
    private bool _isStaggered = false;
    private int _emotionLevel;
    private int _MaxEmotionLevel;
    private int _emotionStack;
    private int _maxEmotionStack;
    private int _activeSpeedSlotCount;
    private int _staggeredTurnsRemaining = 0;
    private readonly CardResolver _cardResolver = new();

    public int CharacterId => _state.CharacterId;
    public Team Team => _state.Team;
    public int BaseSpeedSlotCount => _state.SpeedSlotCount;
    public bool IsDead => _isDead;
    public CardManager CardManager => _cardManager;
    public bool IsStaggered => _isStaggered;
    public int CurrentEnergy => _currentEnergy;
    public int MaxEnergy => _maxEnergy;
    public int CurrentHp => _currentHp;
    public int MaxHp => _maxHp;
    public int CurrentStagger => _currentStagger;
    public int MaxStagger => _maxStagger;
    public int EmotionLevel => _emotionLevel;   
    public int EmotionStack => _emotionStack;
    public int MaxEmotionStack => _state.MaxEmotionStack;
    public int MaxEmotionLevel => _state.MaxEmotionLevel;

    public int EmotionGainOnDamageDealt => _state.EmotionGainOnDamageDealt;
    public int EmotionGainOnDamageReceived => _state.EmotionGainOnDamageReceived;
    public int EmotionGainOnStagger => _state.EmotionGainOnStagger;
    public int EmotionGainOnStaggered => _state.EmotionGainOnStaggered;
    public int EmotionGainOnStaggerHeal => _state.EmotionGainOnStaggerHeal;
    public IReadOnlyList<SpeedSlotRuntime> SpeedSlots => _speedSlots;

    public CharacterRuntime(CharacterState state, IEventSink eventSink, IRng rng)
    {
        _state = state;
        _eventSink = eventSink;

        _maxHp = state.MaxHp;
        _currentHp = _maxHp;

        _maxStagger = state.MaxStagger;
        _currentStagger = _maxStagger;

        _maxEnergy = state.MaxEnergy;
        _currentEnergy = _maxEnergy;

        _cardManager = new CardManager(new List<CardModel>(state.InitialDeck), rng);

        _activeSpeedSlotCount = state.SpeedSlotCount;
        CreateSpeedSlots();

        foreach (var passiveData in state.Passives)
        {
            var passive = PassiveFactory.Create(passiveData, this);
            if (passive != null)
                _passives.Add(passive);
        }
    }

    public void EnqueueEvent(ICombatEvent ev) => _eventSink.EnqueueEvent(ev);
    public void ChangeMaxHp(int amount)
    {
        _maxHp += amount;
        _currentHp += amount;

        if (_maxHp <= 0) _maxHp = 0;
        if (_currentHp > _maxHp) _currentHp = _maxHp;
        if (_currentHp < 0) _currentHp = 0;
    }

    public void ChangeMaxStagger(int amount)
    {
        _maxStagger += amount;
        _currentStagger += amount;

        if(_maxStagger <= 1) _maxStagger = 1;
        if (_currentStagger > _maxStagger) _currentStagger = _maxStagger;
        if (_currentStagger < 0) _currentStagger = 0;
    }

    public void ChangeMaxEnergy(int amount)
    {
        _maxEnergy += amount;
        _currentEnergy += amount;

        if(_maxEnergy <= 1) _maxEnergy = 1;
        if (_currentEnergy > _maxEnergy) _currentEnergy = _maxEnergy;
        if (_currentEnergy < 0) _currentEnergy = 0;
    }

    public void RecoverEnergy(int amount)
    {
        _currentEnergy += amount;
        if (_currentEnergy > _maxEnergy)
        {
            _currentEnergy = _maxEnergy;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHp -= amount;
        if (_currentHp < 0)
            _currentHp = 0;
    }
    public bool ShouldDie() => _currentHp <= 0 && !_isDead;
    public void Die()
    {
        _isDead = true;
    }
    public void TakeStagger(int amount)
    {
        if (_isStaggered) return;

        _currentStagger -= amount;
        if (_currentStagger < 0)
            _currentStagger = 0;
    }
    public bool ShouldEnterStagger() => _currentStagger <= 0 && !_isStaggered;
    public void EnterStagger()
    {
        _isStaggered = true;
        _staggeredTurnsRemaining = 2;
    }
    public void RecoverStagger(int amount)
    {
        _currentStagger += amount;
        if (_currentStagger > _state.MaxStagger)
            _currentStagger = _state.MaxStagger;

        if (_currentStagger > 0 && _isStaggered)
            _isStaggered = false;
    }
    public void GainEmotionStack(int stack) // 나중에 긍정 감정 & 부정 감정 섞이면 로직 고쳐야 함
    {
        _emotionStack += stack;

        if (_emotionStack >= _state.MaxEmotionStack)
        _emotionStack = _state.MaxEmotionStack;
    }

    public void EmotionLevelUp()
    {
        if (_emotionStack < _state.MaxEmotionStack) return;

        _emotionLevel++;
        _emotionStack = 0;
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
    public void ExitStagger()
    {
        _isStaggered = false;
        _currentStagger = _state.MaxStagger; // 게이지 전부 회복
    }
    public bool CanUseAction(ActionInstance action)
    => _cardResolver.CanUse(action.Card, this);
    public void UseEnergy(int amount)
    {
        _currentEnergy -= amount;
        if (_currentEnergy < 0) _currentEnergy = 0;
    }
    public void IncreaseMaxHp(int amount)
    {
        _maxHp += amount;
        _currentHp += amount;
    }

    public void IncreaseMaxStagger(int amount)
    {
        _maxStagger += amount;
        _currentStagger += amount;
    }

    public void IncreaseMaxEnergy(int amount)
    {
        _maxEnergy += amount;
        _currentEnergy += amount;
    }

    public void UseAction(ActionInstance action)
    {
        if (!CanUseAction(action)) return;

        EnqueueEvent(new EnergyUseEvent(CharacterId, action.Card.Cost));

        var entries = _cardResolver.BuildDiceEntries(
            action.Card, action, _state.CharacterId, ref _nextDiceId
        );

        foreach (var entry in entries)
        {
            _diceById[entry.Handle.DiceId] = entry;
            _dicePool.Inject(entry);
        }
    }
    public DiceEntry? Peek() => _dicePool.Peek();
    public void Advance(AdvanceType type) => _dicePool.Advance(type);
    public void RecoverDice() => _dicePool.Recover();
    public void DestroyUsedDice() => _dicePool.DestroyUsed();
    public void ResetDiceForNextTurn() => _dicePool.ResetForNextTurn();

    public int AddStatus(StatusEffectType type, int stack, bool delayed = false)
    {
        if (!_effectMap.TryGetValue(type, out var effect) || effect.IsExpired)
        {
            effect = StatusFactory.Create(type, this, 0);
            _statusEffects.Add(effect);
            _effectMap[type] = effect;
        }

        effect.AddStack(stack, delayed);
        
        _dirty = true;

        return delayed ? effect.PendingStack : effect.Stack;
    }

    public void TriggerTurnStart()
    {
        var ctx = new TurnStartContext(this);

        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnTurnStart(ctx);
        }

        FlushExpired();

        //패시브 효과 트리거
        foreach (var passive in _passives)
            passive.OnTurnStart(ctx);
    }

    public void TriggerBeforeDamage(IDamageContext ctx)
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnBeforeDamage(ctx);
        }

        FlushExpired();

        foreach (var passive in _passives)
            passive.OnBeforeDamage(ctx);
    }

    public void TriggerAfterDamage(IDamageContext ctx)
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnAfterDamage(ctx);
        }
            
        FlushExpired();

        foreach (var passive in _passives)
            passive.OnAfterDamage(ctx);
    }
    public void TriggerBeforeClash(ClashContext ctx, bool isOwnerA)
    {
        // 상태이상
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnBeforeClash(ctx, isOwnerA);
        }
            
        FlushExpired();

        // 패시브
        foreach (var passive in _passives)
            passive.OnBeforeClash(ctx, isOwnerA);
    }

    public void TriggerDiceRoll()
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnDiceRoll();
        }

        FlushExpired();
    }

    public void TriggerTurnEnd()
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.TickTurnEnd();
        }

        FlushExpired();

        if (_emotionStack >= _state.MaxEmotionStack && _emotionLevel < _state.MaxEmotionLevel)
        {
            EnqueueEvent(new EmotionLevelUpEvent(CharacterId)); //감정 레벨 올리는 이벤트 만들기
        }

        if (_isStaggered)
        {
            _staggeredTurnsRemaining--;
            if (_staggeredTurnsRemaining <= 0)
                EnqueueEvent(new StaggerExitEvent(CharacterId));
        }
    }
    public void TriggerBeforeStagger(StaggerContext ctx)
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnBeforeStagger(ctx);
        }

        FlushExpired();

        foreach (var passive in _passives)
            passive.OnBeforeStagger(ctx);
    }

    public void TriggerAfterStagger(StaggerContext ctx)
    {
        EnsureSorted();

        var effects = _statusEffects.ToArray();
        foreach (var effect in effects)
        {
            if (effect.IsExpired) continue;
            effect.OnAfterStagger(ctx);
        }

        FlushExpired();

        foreach (var passive in _passives)
            passive.OnAfterStagger(ctx);
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
            var expired = _statusEffects[i];

            if (!expired.IsExpired) continue;

            _statusEffects.RemoveAt(i);
            if (_effectMap.TryGetValue(expired.Type, out var cur) && ReferenceEquals(expired, cur))
            {
                _effectMap.Remove(expired.Type);
            }
        }
    }
}