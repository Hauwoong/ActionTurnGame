public class BattleEndEvent : ICombatEvent
{
    public void Apply(BattleRuntime runtime)
    {
        runtime.AddLog(new BattleEndLog());
    }
}