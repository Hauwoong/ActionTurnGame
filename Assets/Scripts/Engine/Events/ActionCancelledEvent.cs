public class ActionCancelledEvent : ICombatEvent
{
    private readonly SpeedSlot _slot;

    public ActionCancelledEvent(SpeedSlot slot)
    {
        _slot = slot;
    }

    public void Apply(BattleRuntime runtime)
    {
        if (runtime.BoutGraph.ActionBySlot.TryGetValue(_slot, out var action))
        {
            runtime.BoutGraph.CancelAction(action);

            runtime.EnqueueEvent(new ReturnCardEvent(_slot.CharacterId, action.Card));

            runtime.AddLog(new ActionCancelledLog(
                action.ActionId,
                _slot.CharacterId, _slot.SlotIndex
            ));
        }
    }
}
