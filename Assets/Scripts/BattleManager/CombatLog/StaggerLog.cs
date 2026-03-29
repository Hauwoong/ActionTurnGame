public class StaggerLog : CombatLog
{
    public int CharacterId { get; }
    public int Amount { get; }
    public bool IsRecover { get; }

    public StaggerLog(int characterId, int amount, bool isRecover)
    {
        CharacterId = characterId;
        Amount = amount;
        IsRecover = isRecover;
    }
}