
public class AttackBoostPassive : PassiveEffect
{
    private readonly int _amount;

    public AttackBoostPassive(CharacterRuntime owner, int amount)
        : base(owner, PassiveType.AttackBoost)
    {
        _amount = amount;
    }

    public override void OnModifyRoll(DiceRollContext ctx)
    {
        if (!ctx.Dice.Type.IsOffensive()) return;

        ctx.ModifiedRoll += _amount;
    }
}