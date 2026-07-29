// 출혈 - 주사위 굴릴 때마다 스택만큼 피해 + 스택 절반 감소
public class BleedEffect : StatusEffectRuntime
{
    public BleedEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, duration: Permanent, priority: 0, StatusEffectType.Bleed) { }

    public override void OnDiceRoll()
    {
        if (Stack <= 0) return;
        var ctx = new StatusDamageContext(Owner, Stack);
        Stack = Stack / 2;
        if (Stack <= 0 && PendingStack <= 0) IsExpired = true;
        Owner.EnqueueEvent(new StatusDamageEvent(ctx));
    }
}

// 화상 - 턴 종료 시 스택만큼 피해 + 스택 절반 감소
public class BurnEffect : StatusEffectRuntime
{
    public BurnEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, duration: Permanent,  priority: 1, StatusEffectType.Burn) { }

    protected override void OnTurnEnd()
    {
        if (Stack <= 0) return;
        var ctx = new StatusDamageContext(Owner, Stack);
        Stack = Stack / 2;
        if (Stack <= 0 && PendingStack <= 0) IsExpired = true;
        Owner.EnqueueEvent(new StatusDamageEvent(ctx));
    }
}

// 힘 - 데미지 계산 전 스택만큼 데미지 증가
public class StrengthEffect : StatusEffectRuntime
{
    public StrengthEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, duration: 1, priority: 2, StatusEffectType.Strength) { }

    public override void OnModifyRoll(DiceRollContext ctx)
    {
        if (ctx.Dice.Type != DiceType.Attack) return;

        ctx.ModifiedRoll += Stack;
    }
}

// 마비 - 데미지 계산 전 스택만큼 데미지 감소
public class ParalysisEffect : StatusEffectRuntime
{
    public ParalysisEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, duration: 1,  priority: 3, StatusEffectType.Paralysis) { }

    public override void OnModifyRoll(DiceRollContext ctx)
    {
        ctx.ModifiedRoll -= Stack;
    }
}