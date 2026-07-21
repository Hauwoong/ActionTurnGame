public class BoutStartEvent : ICombatEvent
{
    public ActionInstance A { get; }
    public ActionInstance B { get; }
    public int AttackerId { get; }
    public int TargetId { get; }

    public BoutStartEvent(ActionInstance a, ActionInstance b, int attackerId, int targetId)
    {
        A = a;
        B = b;
        AttackerId = attackerId;
        TargetId = targetId;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.AddLog(new BoutStartLog(AttackerId, TargetId));
        runtime.Executor.ResolveCombat(A, B, AttackerId, TargetId);
        runtime.EnqueueEvent(new BoutEndEvent(AttackerId, TargetId, B?.SourceSlot.CharacterId));
    }
}