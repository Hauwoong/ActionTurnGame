public class AttackBoostData : PassiveData
{
    public int Amount { get; }

    public AttackBoostData(int amount)
        : base("Attack Boost", PassiveType.AttackBoost)
    {
        Amount = amount;
    }
}