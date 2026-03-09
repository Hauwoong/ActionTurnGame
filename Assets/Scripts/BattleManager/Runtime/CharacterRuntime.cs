using System.Collections.Generic;

public class CharacterRuntime
{
    private readonly CharacterState state; // 기존의 character을 characterstate로 변경

    private readonly List<DiceEntry> DicePool = new(); // 캐릭터 런타임이 소유하는 주사위
    private readonly Dictionary<int, DiceRuntime> DiceById = new(); // 이벤트가 주사위 id 추적을 용이하게 하기 위한 딕셔너리

    public int DiceCursor { get; private set; } // List + Cursor 조합으로 주사위 관리

    private int CurrentHp;

    private readonly List<StatusEffectRuntime> _statusEffects = new(); // 이벤트 개입을 전제로 하는 상태이상

    private int NextDiceId = 0; // CharacterRuntime -> DiceEntry 소유하니 id 생성은 characterruntime 역할

    private readonly List<SpeedSlotRuntime> speedSlots = new(); // 캐릭터 런타임이 가지고 있는 스피드 슬록 => 스피드 슬롯 구조체 에서 스피드 슬롯 런타임으로 교체 예정
    public IReadOnlyList<SpeedSlotRuntime> SpeedSlots => speedSlots; // 캐릭터 스피드 슬롯 캡슐화

    public bool IsFinished => DiceCursor >= DicePool.Count;

    private readonly Dictionary<StatusEffectType, StatusEffectRuntime> Effects = new();

    public bool _dirty;

    public CharacterRuntime(CharacterState owner)
    {
        state = owner;
        DiceCursor = 0;
        CurrentHp = state.MaxHp;
        CreateSpeedSlots();
    }

    void CreateSpeedSlots()
    {
        for (int i = 0; i < state.SpeedSlotCount; i++)
        {
            var speedSlot = new SpeedSlot(state.CharacterId, i);

            var runtime = new SpeedSlotRuntime(speedSlot, state.MinSpeed, state.MaxSpeed);

            speedSlots.Add(runtime);
        }
    }

    public IReadOnlyList<DiceEntry> GetRemainingDice()
    {
        return DicePool.GetRange(DiceCursor, DicePool.Count - DiceCursor);
    }

    public bool TryGetCurrentDice(out DiceEntry dice)
    {
        if (IsFinished)
        {
            dice = default;
            return false;
        }

        dice = DicePool[DiceCursor];
        return true;
    }

    public DiceRuntime GetDice(int id)
    {
        if (DiceById.TryGetValue(id, out DiceRuntime runtime))
        {
            return runtime;
        }

        return null;
    }

    public void UseCard(CardData card)
    {
        var cardDice = card.dices;
        var dices = CreateDiceEntry(cardDice);
        AddCardDice(dices);
    }

    List<DiceEntry> CreateDiceEntry(List<DiceData> cardDice)
    {
        List<DiceEntry> dices = new();

        foreach (var dice in cardDice)
        {
            int id = NextDiceId++;

            var diceRuntime = new DiceRuntime
            {
                Type = dice.type,
                Min = dice.min,
                Max = dice.max,
                IsDestroyed = false
            };

            var characterhandle = new CharacterHandle(state.CharacterId);

            var diceHandle = new DiceHandle(
                characterhandle, id);

            DiceById[id] = diceRuntime;

            dices.Add(new DiceEntry(diceRuntime, diceHandle));
        }

        return dices;
    }

    void AddCardDice(List<DiceEntry> dices) // ActionBySlot 에서 카드 사용 -> 그럼 카드 안에 주사위 리스트는 여기서 계산 해야 하나? 
    {
        DicePool.InsertRange(DiceCursor, dices);
    }
    
    public void MarkDestroyed(int DiceId)
    {
        if (DiceById.TryGetValue(DiceId, out var dice))
        {
            dice.IsDestroyed = true;
        }
    }

    public void Advance()
    {
        while (!IsFinished &&
                DicePool[DiceCursor].Dice.IsDestroyed)
        {
            DiceCursor++;
        }
    }

    public void ResetCursor()
    {
        DiceCursor = 0;
    }

    public void AddStatus(StatusEffectType type, int stack)
    {
        if (Effects.TryGetValue(type, out StatusEffectRuntime effect))
        {
            effect.AddStack(stack);
            _dirty = true;
        }

        else
        {
            var newEffect = StatusFactory.Create(type, this, stack); // 여기서 바로 characterRuntime이 상태이상 런타임 생성 VS StatusFactory를 이용하여 생성
            _statusEffects.Add(newEffect);
            Effects[type] = newEffect;
            _dirty = true;
        }
    }

    public void TriggerTurnStart()
    {
        var ctx = new TurnStartContext(this);

        EnsureSorted();

        foreach (var effect in _statusEffects)
        {
            effect.OnTurnStart(ctx);
        }

        FlushExpired();

    }

    public void TriggerBeforeDamage(DamageContext ctx)
    {
        EnsureSorted();

        foreach (var effect in _statusEffects)
        {
            effect.OnBeforeDamage(ctx);
        }

        FlushExpired();

    }

    public void TriggerAfterDamage(DamageContext ctx)
    {
        EnsureSorted();

        foreach (var effect in _statusEffects)
        {
            effect.OnAfterDamage(ctx);
        }

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
