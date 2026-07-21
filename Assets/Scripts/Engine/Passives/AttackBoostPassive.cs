
public class AttackBoostPassive : PassiveEffect
{
    private readonly int _amount;

    public AttackBoostPassive(CharacterRuntime owner, int amount)
        : base(owner, PassiveType.AttackBoost)
    {
        _amount = amount;
    }

    public override void OnBeforeClash(ClashContext ctx, bool isOwnerA)
    {
        // 공격 주사위일 때만 적용
        if (isOwnerA && ctx.DiceA.Type == DiceType.Attack)
            ctx.ModifiedRollA += _amount;
        else if (!isOwnerA && ctx.DiceB.Type == DiceType.Attack)
            ctx.ModifiedRollB += _amount;
    }
}