public class ChangeMaxStaggerLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxStaggerLog(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }
}