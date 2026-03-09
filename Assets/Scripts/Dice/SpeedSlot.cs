[System.Serializable]
public struct SpeedSlot
{
    public int CharacterId;
    public int SlotIndex;

    public SpeedSlot(int characterId, int slotIndex)
    {
        CharacterId = characterId;
        SlotIndex = slotIndex;
    }
}