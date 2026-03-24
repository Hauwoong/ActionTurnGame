public class DiceClashLog : CombatLog
{
    public DiceHandle HandleA { get; }
    public DiceHandle HandleB { get; }
    public int RollA { get; }
    public int RollB { get; }
    public ClashResult Result { get; }
    public AdvanceType AdvanceA { get; }
    public AdvanceType AdvanceB { get; }

    public DiceClashLog(DiceHandle handleA, DiceHandle handleB, int rollA, int rollB, ClashResult result, AdvanceType advanceA, AdvanceType advanceB)
    {
        HandleA = handleA;
        HandleB = handleB;
        RollA = rollA;
        RollB = rollB;
        Result = result;
        AdvanceA = advanceA;
        AdvanceB = advanceB;
    }
}
