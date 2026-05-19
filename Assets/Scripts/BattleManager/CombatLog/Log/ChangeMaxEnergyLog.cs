public class ChangeMaxEnergyLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxEnergyLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}
