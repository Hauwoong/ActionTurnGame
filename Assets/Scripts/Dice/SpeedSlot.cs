
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpeedSlot
{
    public Character owner;
    public int index;

    // ±¼¸² ¼Óµµ
    public int speed => owner.rolledSpeeds[index];

    // ½½·Ô »ç¿ë À¯¹«
    private bool isUsed;
    public bool IsUsed => isUsed;

    public void Use()
    {
        isUsed = true;
    }
}
   

