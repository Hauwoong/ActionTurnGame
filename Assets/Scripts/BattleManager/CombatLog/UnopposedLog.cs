public class UnopposedLog : CombatLog
{
    public DiceHandle Handle { get; }
    public DiceType DiceType { get; }
    public AdvanceType Advance { get; }

    public UnopposedLog(DiceHandle handle, DiceType diceType, AdvanceType advance)
    {
        Handle = handle;
        DiceType = diceType;
        Advance = advance;
    }
}