public class UnopposedLog : CombatLog
{
    public DiceHandle Handle { get; }
    public int TargetId { get; }
    public int BaseRoll { get; }
    public int ModifiedRoll { get; }

    public UnopposedLog(DiceHandle handle, int targetId, int baseRoll, int modifiedRoll)
    {
        Handle = handle;
        TargetId = targetId;
        BaseRoll = baseRoll;
        ModifiedRoll = modifiedRoll;
    }
}