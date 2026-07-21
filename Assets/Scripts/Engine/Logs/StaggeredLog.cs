
public class StaggeredLog : CombatLog
{
    public int CharacterId { get; }
    
    public StaggeredLog(int characterId)
    {
        CharacterId = characterId;
    }
}