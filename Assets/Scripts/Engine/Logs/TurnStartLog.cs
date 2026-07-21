// 턴 시작
public class TurnStartLog : CombatLog
{
    public int CharacterId { get; }

    public TurnStartLog(int characterId)
    {
        CharacterId = characterId;
    }
}