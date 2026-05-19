public class ChangeMaxHpLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxHpLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}