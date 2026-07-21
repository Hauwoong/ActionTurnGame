// 감정 레벨업
public class EmotionLevelUpLog : CombatLog
{
    public int CharacterId { get; }
    public int NewLevel { get; }

    public EmotionLevelUpLog(int characterId, int newLevel)
    {
        CharacterId = characterId;
        NewLevel = newLevel;
    }
}