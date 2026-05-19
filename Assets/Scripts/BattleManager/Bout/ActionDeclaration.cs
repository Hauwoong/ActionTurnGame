
public class ActionDeclaration
{
    public SpeedSlot Slot { get; }
    public SpeedSlot Target {  get; }
    public CardData Card {  get; }
    public int RegisterOrder { get; }
    public ActionDeclaration(SpeedSlot slot, SpeedSlot target, CardData card, int registerOrder)
    {
        Slot = slot;
        Target = target;
        Card = card;
        RegisterOrder = registerOrder;
    }
}
