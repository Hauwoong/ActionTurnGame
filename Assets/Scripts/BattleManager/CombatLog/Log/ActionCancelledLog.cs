public class ActionCancelledLog : CombatLog
{
    public int ActionId { get; }
    public int CharacterId { get; }
    public int SlotIndex { get; }

    public ActionCancelledLog(int actionId, int characterId, int slotIndex)
    {
        ActionId = actionId;
        CharacterId = characterId;
        SlotIndex = slotIndex;
    }
}