public class BoutEndEvent : ICombatEvent
{
    public int AttackerId { get; }
    public int TargetId { get; }
    public int? DefenderId { get; }

    public BoutEndEvent(int attackerId, int targetId, int? defenderId)
    {
        AttackerId = attackerId;
        TargetId = targetId;
        DefenderId = defenderId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var attacker = runtime.GetCharacterRuntime(AttackerId);
        var target = runtime.GetCharacterRuntime(TargetId);

        attacker.EndBoutDice();
        target.EndBoutDice();

        runtime.AddLog(new BoutEndLog(AttackerId, TargetId));
    }
}