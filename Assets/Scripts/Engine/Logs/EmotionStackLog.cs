public class EmotionStackLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public EmotionStackLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}