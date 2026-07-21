public class ActionRegisteredEvent : ICombatEvent
{
    private readonly SpeedSlot _sourceSlot;
    private readonly SpeedSlot _targetSlot;
    private readonly CardData _card;

    public ActionRegisteredEvent(SpeedSlot sourceSlot, SpeedSlot targetSlot, CardData card)
    {
        _sourceSlot = sourceSlot;
        _targetSlot = targetSlot;
        _card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        int actionId = runtime.NextActionId();
        var action = new ActionInstance(_sourceSlot, _targetSlot, _card, actionId);
        runtime.BoutGraph.RegisterAction(action);

        runtime.AddLog(new ActionRegisteredLog(
            actionId,
            _sourceSlot.CharacterId, _sourceSlot.SlotIndex,
            _targetSlot.CharacterId, _targetSlot.SlotIndex,
            _card
        ));
    }
}
