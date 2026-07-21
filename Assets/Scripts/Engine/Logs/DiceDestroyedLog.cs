// 주사위 파괴
public class DiceDestroyedLog : CombatLog
{
    public DiceHandle Handle { get; }

    public DiceDestroyedLog(DiceHandle handle)
    {
        Handle = handle;
    }
}
