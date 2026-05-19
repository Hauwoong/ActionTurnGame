public class DeathLog : CombatLog
{
    public int CharacterId { get; }

    public DeathLog(int characterId)
    {
        CharacterId = characterId;
    }
}