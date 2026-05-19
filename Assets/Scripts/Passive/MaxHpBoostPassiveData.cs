
public class MaxHpBoostPassiveData : PassiveData, IStatModifierPassive
{
    public int Amount { get; }

    public MaxHpBoostPassiveData(int amount)
        : base("Max Hp Boost", PassiveType.MaxHpBoost)
    {
        Amount = amount;
    }

    public void Apply(CharacterStateBuilder builder)
        => builder.MaxHp += Amount;
}