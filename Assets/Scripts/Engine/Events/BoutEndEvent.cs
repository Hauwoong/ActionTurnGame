public class BoutEndEvent : ICombatEvent
{
    public int AttackerId { get; }
    public int TargetId { get; }
    public bool WasClash { get; }

    public BoutEndEvent(int attackerId, int targetId, bool wasClash)
    {
        AttackerId = attackerId;
        TargetId = targetId;
        WasClash = wasClash;
    }

    public void Apply(BattleRuntime runtime)
    {
        var attacker = runtime.GetCharacterRuntime(AttackerId);
        var target = runtime.GetCharacterRuntime(TargetId);

        attacker.EndBoutDice();
        target.EndBoutDice();

        runtime.AddLog(new BoutEndLog(AttackerId, TargetId, WasClash));
    }
}