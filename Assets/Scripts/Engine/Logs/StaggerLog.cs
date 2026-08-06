public class StaggerLog : CombatLog
{
    public int AttackerId { get; }
    public int CharacterId { get; }
    public int Amount { get; }
    public bool IsRecover { get; }

    public StaggerLog(int attackerId, int characterId, int amount, bool isRecover)
    {
        AttackerId = attackerId;
        CharacterId = characterId;
        Amount = amount;
        IsRecover = isRecover;
    }
}