
public class ActionInstance
{
    public SpeedSlot SourceSlot { get; }
    public SpeedSlot TargetSlot { get; }
    public CardData Card { get; }
    public int ActionId { get; }
    
    public ActionInstance(SpeedSlot source, SpeedSlot target, CardData card, int actionId)
    {
        SourceSlot = source;
        TargetSlot = target;
        Card = card;
        ActionId = actionId;
    }
}
