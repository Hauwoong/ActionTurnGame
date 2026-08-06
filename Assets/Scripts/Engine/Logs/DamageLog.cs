public class DamageLog : CombatLog
{
    public int AttackerId { get; }
    public int TargetId { get; }
    public int Amount { get; }

    public DamageLog(int attackerId, int targetId, int amount)
    {
        AttackerId = attackerId;
        TargetId = targetId;
        Amount = amount;
    }
}