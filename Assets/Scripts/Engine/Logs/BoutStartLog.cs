public class BoutStartLog : CombatLog
{
    public int AttackerId { get; }
    public int TargetId { get; }
    public bool WasClash { get; }

    public BoutStartLog(int attackerId, int targetId, bool wasClash)
    {
        AttackerId = attackerId;
        TargetId = targetId;
        WasClash = wasClash;
    }
}