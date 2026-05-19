public class BoutStartLog : CombatLog
{
    public int AttackerId { get; }
    public int TargetId { get; }

    public BoutStartLog(int attackerId, int targetId)
    {
        AttackerId = attackerId;
        TargetId = targetId;
    }
}