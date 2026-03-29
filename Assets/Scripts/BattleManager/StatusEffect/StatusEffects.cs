// 출혈 - 주사위 합마다 스택만큼 피해 + 스택 절반 감소
public class BleedEffect : StatusEffectRuntime
{
    public BleedEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, priority: 0) { }

    public override void OnDiceClash()
    {
        Owner.EnqueueEvent(new StatusDamageEvent(Owner.CharacterId, Stack));
        Stack = Stack / 2;
        if (Stack <= 0)
            IsExpired = true;
    }
}

// 화상 - 턴 종료 시 스택만큼 피해 + 스택 절반 감소
public class BurnEffect : StatusEffectRuntime
{
    public BurnEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, priority: 1) { }

    public override void OnTurnEnd()
    {
        Owner.EnqueueEvent(new StatusDamageEvent(Owner.CharacterId, Stack));
        Stack = Stack / 2;
        if (Stack <= 0)
            IsExpired = true;
    }
}

// 힘 - 데미지 계산 전 스택만큼 데미지 증가
public class StrengthEffect : StatusEffectRuntime
{
    public StrengthEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, priority: 2) { }

    public override void OnBeforeClash(ClashContext ctx, bool isOwnerA)
    {
        if (isOwnerA)
            ctx.ModifiedRollA += Stack;
        else
            ctx.ModifiedRollB += Stack;
    }
}

// 마비 - 데미지 계산 전 스택만큼 데미지 감소
public class ParalysisEffect : StatusEffectRuntime
{
    public ParalysisEffect(CharacterRuntime owner, int stack)
        : base(owner, stack, priority: 3) { }

    public override void OnBeforeClash(ClashContext ctx, bool isOwnerA)
    {
        if (isOwnerA)
            ctx.ModifiedRollA -= Stack;
        else
            ctx.ModifiedRollB -= Stack;
    }
}