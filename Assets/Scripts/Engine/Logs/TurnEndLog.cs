// 턴 종료
public class TurnEndLog : CombatLog
{
    public int CharacterId { get; }

    public TurnEndLog(int characterId)
    {
        CharacterId = characterId;
    }
}