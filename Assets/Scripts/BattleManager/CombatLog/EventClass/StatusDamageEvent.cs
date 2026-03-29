public class StatusDamageEvent : ICombatEvent
{
    public int DefenderId { get; }
    public int Amount { get; }

    public StatusDamageEvent(int defenderId, int amount)
    {
        DefenderId = defenderId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var defender = runtime.GetCharacterRuntime(DefenderId);
        defender.TakeDamage(Amount);
        runtime.AddLog(new StatusDamageLog(DefenderId, Amount));
    }
}