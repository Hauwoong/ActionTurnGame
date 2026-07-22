public class ActionRegisteredLog : CombatLog
{
    public int ActionId { get; }
    public int CharacterId { get; }
    public int SlotIndex { get; }
    public int TargetCharacterId { get; }
    public int TargetSlotIndex { get; }
    public CardModel Card { get; }

    public ActionRegisteredLog(int actionId, int characterId, int slotIndex,
        int targetCharacterId, int targetSlotIndex, CardModel card)
    {
        ActionId = actionId;
        CharacterId = characterId;
        SlotIndex = slotIndex;
        TargetCharacterId = targetCharacterId;
        TargetSlotIndex = targetSlotIndex;
        Card = card;
    }
}
