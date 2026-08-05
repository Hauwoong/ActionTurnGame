public class DiceClashLog : CombatLog
{
    public DiceHandle HandleA { get; }
    public DiceHandle HandleB { get; }
    public int BaseRollA { get; }
    public int BaseRollB { get; }
    public int ModifiedRollA { get; }
    public int ModifiedRollB { get; }
    public AdvanceType AdvanceA { get; }
    public AdvanceType AdvanceB { get; }
    public ClashResult Result { get; }

    public DiceClashLog(DiceHandle handleA, DiceHandle handleB,
        int baseRollA, int baseRollB, int modifiedRollA, int modifiedRollB,
        AdvanceType advanceA, AdvanceType advanceB, ClashResult result)
    {
        HandleA = handleA;
        HandleB = handleB;
        BaseRollA = baseRollA;
        BaseRollB = baseRollB;
        ModifiedRollA = modifiedRollA;
        ModifiedRollB = modifiedRollB;
        AdvanceA = advanceA;
        AdvanceB = advanceB;
        Result = result;
    }
}
