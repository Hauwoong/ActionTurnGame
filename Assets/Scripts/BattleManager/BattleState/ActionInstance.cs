
public class ActionInstance
{
    public SpeedSlot SourceSlot { get; }
    public SpeedSlot TargetSlot { get; }
    public CardData Card { get; }

    public int RegisterOrder { get; }
    
    public ActionInstance(SpeedSlot source, SpeedSlot target, CardData card, int registerOrder)
    {
        SourceSlot = source;
        TargetSlot = target;
        Card = card;
        RegisterOrder = registerOrder;
    }
}
