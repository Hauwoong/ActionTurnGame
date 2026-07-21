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
        runtime.EnqueueEvent(new DiceRecoverEvent(AttackerId));
        runtime.EnqueueEvent(new DiceDestroyUsedEvent(AttackerId));

        if (DefenderId.HasValue)
        {
            runtime.EnqueueEvent(new DiceRecoverEvent(DefenderId.Value));
            runtime.EnqueueEvent(new DiceDestroyUsedEvent(DefenderId.Value));
        }

        runtime.AddLog(new BoutEndLog(AttackerId, TargetId));
    }
}