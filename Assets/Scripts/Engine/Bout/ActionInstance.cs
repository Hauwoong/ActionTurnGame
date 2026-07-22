
public class ActionInstance
{
    public SpeedSlot SourceSlot { get; }
    public SpeedSlot TargetSlot { get; }
    public CardModel Card { get; }
    public int ActionId { get; }
    
    public ActionInstance(SpeedSlot source, SpeedSlot target, CardModel card, int actionId)
    {
        SourceSlot = source;
        TargetSlot = target;
        Card = card;
        ActionId = actionId;
    }
}
