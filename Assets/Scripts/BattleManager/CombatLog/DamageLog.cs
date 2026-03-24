public class DamageLog : CombatLog
{
    public int TargetId { get; }
    public int Amount { get; }

    public DamageLog(int targetId, int amount)
    {
        TargetId = targetId;
        Amount = amount;
    }
}