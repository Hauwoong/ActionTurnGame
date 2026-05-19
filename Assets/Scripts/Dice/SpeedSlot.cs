using System;

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

    public override bool Equals(object obj)
    {
        if (obj is SpeedSlot other)
        {
            return CharacterId == other.CharacterId && SlotIndex == other.SlotIndex;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CharacterId, SlotIndex);
    }

    public static bool operator == (SpeedSlot a, SpeedSlot b)
    {
        return a.CharacterId == b.CharacterId && a.SlotIndex == b.SlotIndex;
    }

    public static bool operator != (SpeedSlot a, SpeedSlot b)
    {
        return !(a == b);
    }
}