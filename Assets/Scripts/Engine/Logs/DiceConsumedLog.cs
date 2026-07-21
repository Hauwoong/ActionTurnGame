// 주사위 소비
public class DiceConsumedLog : CombatLog
{
    public DiceHandle Handle { get; }

    public DiceConsumedLog(DiceHandle handle)
    {
        Handle = handle;
    }
}