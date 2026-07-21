// 주사위 회복
public class DiceRecoverLog : CombatLog
{
    public int CharacterId { get; }

    public DiceRecoverLog(int characterId)
    {
        CharacterId = characterId;
    }
}