using System;
public struct ActionPriority : IComparable<ActionPriority>
{
    public int Speed;
    public int CharacterId;
    public int SlotIndex;

    public int CompareTo(ActionPriority other)
    {
        int speedCompare = other.Speed.CompareTo(Speed);
        if (speedCompare != 0) return speedCompare;

       int characterCompare = CharacterId.CompareTo(other.CharacterId);
        if (characterCompare != 0) return characterCompare;

        return SlotIndex.CompareTo(other.SlotIndex);
    }
}
