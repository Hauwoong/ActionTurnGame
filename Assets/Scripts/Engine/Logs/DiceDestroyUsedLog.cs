public class DiceDestroyUsedLog : CombatLog
{
    public int CharacterId { get; }

    public DiceDestroyUsedLog(int characterId)
    {
        CharacterId = characterId;
    }
}