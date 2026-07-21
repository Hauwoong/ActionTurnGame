public class BattleEndLog : CombatLog
{
    // 승리 팀. 양측 동시 전멸이면 null.
    public Team? Winner { get; }

    public BattleEndLog(Team? winner)
    {
        Winner = winner;
    }
}
