public class BoutEndLog : CombatLog
{
    public int AttackerId { get; }
    public int TargetId { get; }

    public BoutEndLog(int attackerId, int targetId)
    {
        AttackerId = attackerId;
        TargetId = targetId;
    }
}