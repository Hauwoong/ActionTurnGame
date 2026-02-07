using UnityEngine;

[System.Serializable]
public class SpeedSlot
{
    public Character owner;
    public int index;

    public int speed => owner.rolledSpeeds[index];

    public bool IsUsed => owner.IsSlotUsed(index);

    public void Use()
    {
        owner.UseSlot(index);
    }
}
   

