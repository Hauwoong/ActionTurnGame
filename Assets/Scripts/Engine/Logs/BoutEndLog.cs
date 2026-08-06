public class BoutEndLog : CombatLog
{
    public int AttackerId { get; }
    public int TargetId { get; }
    public bool WasClash { get; }

    public BoutEndLog(int attackerId, int targetId, bool wasClash)
    {
        AttackerId = attackerId;
        TargetId = targetId;
        WasClash = wasClash;
    }
}