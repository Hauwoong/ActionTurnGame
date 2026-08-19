
public class MaxHpBoostModel : PassiveModel, IStatModifierPassive
{
    public int Amount { get; }
    public MaxHpBoostModel(int amount)
    {
        Amount = amount;
    }
    public override PassiveEffect CreateEffect(CharacterRuntime owner)
        => null;
    public void Apply(CharacterStateBuilder builder) => builder.MaxHp += Amount;
}
