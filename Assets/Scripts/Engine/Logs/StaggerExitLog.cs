public class StaggerExitLog : CombatLog
{
    public int CharacterId { get; }

    public StaggerExitLog(int characterId)
    {
        CharacterId = characterId;
    }
}