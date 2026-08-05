
public class DiceReusedLog : CombatLog
{
    public DiceHandle Handle { get; }

    public DiceReusedLog(DiceHandle handle)
    {
        Handle = handle;
    }
}
