using System;
public struct ActionPriority : IComparable<ActionPriority>
{
    public int Speed;
    public int CharacterId;
    public int SlotIndex;

    public int CompareTo(ActionPriority other)
    {
        int speedCompare = Speed.CompareTo(other.Speed);
        if (speedCompare != 0) return speedCompare;

       int characterCompare = other.CharacterId.CompareTo(CharacterId);
        if (characterCompare != 0) return characterCompare;

        return other.SlotIndex.CompareTo(SlotIndex);
    }
}
