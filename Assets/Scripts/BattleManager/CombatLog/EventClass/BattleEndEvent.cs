public class BattleEndEvent : ICombatEvent
{
    // 승리 팀. 양측 동시 전멸이면 null.
    public Team? Winner { get; }

    public BattleEndEvent(Team? winner)
    {
        Winner = winner;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.AddLog(new BattleEndLog(Winner));
    }
}
