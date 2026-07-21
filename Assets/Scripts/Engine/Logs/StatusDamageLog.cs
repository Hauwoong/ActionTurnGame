public class StatusDamageLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public StatusDamageLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}