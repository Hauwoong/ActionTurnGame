public class ActionRegisteredEvent : ICombatEvent
{
    private readonly SpeedSlot _sourceSlot;
    private readonly SpeedSlot _targetSlot;
    private readonly CardModel _card;

    public ActionRegisteredEvent(SpeedSlot sourceSlot, SpeedSlot targetSlot, CardModel card)
    {
        _sourceSlot = sourceSlot;
        _targetSlot = targetSlot;
        _card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        if (runtime.BoutGraph.ActionBySlot.TryGetValue(_sourceSlot, out var current))
            runtime.EnqueueEvent(new ReturnCardEvent(_sourceSlot.CharacterId, current.Card));

        int actionId = runtime.NextActionId();

        var action = new ActionInstance(_sourceSlot, _targetSlot, _card, actionId);

        runtime.BoutGraph.RegisterAction(action);

        runtime.EnqueueEvent(new UseCardEvent(_sourceSlot.CharacterId,_card));

        runtime.AddLog(new ActionRegisteredLog(
            actionId,
            _sourceSlot.CharacterId, _sourceSlot.SlotIndex,
            _targetSlot.CharacterId, _targetSlot.SlotIndex,
            _card
        ));
    }
}
