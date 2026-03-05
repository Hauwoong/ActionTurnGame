using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;

public class CharacterRuntime
{
    private readonly CharacterState state; // 기존의 character을 characterstate로 변경

    private readonly List<DiceEntry> DicePool = new(); // 캐릭터 런타임이 소유하는 주사위
    private readonly Dictionary<int, DiceRuntime> DiceById = new(); // 이벤트가 주사위 id 추적을 용이하게 하기 위한 딕셔너리

    public int DiceCursor { get; private set; } // List + Cursor 조합으로 주사위 관리

    private int CurrentHp;

    private readonly List<StatusEffectRuntime> _statusEffects; // 이벤트 개입을 전제로 하는 상태이상

    private int NextDiceId = 0; // CharacterRuntime -> DiceEntry 소유하니 id 생성은 characterruntime 역할

    public bool IsFinished => DiceCursor >= DicePool.Count;

    public CharacterRuntime(CharacterState owner, int id)
    {
        state = owner;
        CharacterId = id;
        DiceCursor = 0;
        CurrentHp = state.MaxHp;
    }

    public IReadOnlyList<DiceEntry> GetRemainingDice() => DicePool;

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
                IsDestoryed = false
            };

            var diceHandle = new DiceHandle(
                state.Source, id);

            DiceById[id] = diceRuntime;

            dices.Add(new DiceEntry(diceRuntime, diceHandle));
        }

        return dices;
    }

    void AddCardDice(List<DiceEntry> dices) // ActionBySlot 에서 카드 사용 -> 그럼 카드 안에 주사위 리스트는 여기서 계산 해야 하나? 
    {
        DicePool.InsertRange(DiceCursor, dices);
    }
    
    public void MarkDestoryed(int DiceId)
    {
        if (DiceById.TryGetValue(DiceId, out var dice))
        {
            dice.IsDestoryed = true;
        }
    }

    public void Advance()
    {
        while (!IsFinished &&
                DicePool[DiceCursor].Dice.IsDestoryed)
        {
            DiceCursor++;
        }
    }

    public void ResetCursor()
    {
        DiceCursor = 0;
    }

    public void RaiseTurnStart()
    {
        var ctx = new TurnStartContext(this);

        foreach (var effect in _statusEffects)
        {
            effect.OnTurnStart(ctx);
        }

        _statusEffects.RemoveAll(e => e.IsExpird);
    }

    public void RaiseBeforeDamage(DamageContext ctx)
    {
        foreach (var effect in _statusEffects)
        {
            effect.OnBeforeDamage(ctx);
        }

        _statusEffects.RemoveAll(e => e.IsExpird);
    }

    public void RaiseAfterDamage(DamageContext ctx)
    {
        foreach (var effect in _statusEffects)
        {
            effect.OnAfterDamage(ctx);
        }

        _statusEffects.RemoveAll(e => e.IsExpird);
    }
}
