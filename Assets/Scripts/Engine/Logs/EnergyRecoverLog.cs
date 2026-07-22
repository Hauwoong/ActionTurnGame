
public class EnergyRecoverLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public EnergyRecoverLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}
