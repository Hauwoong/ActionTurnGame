public class EnergyUseLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public EnergyUseLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}