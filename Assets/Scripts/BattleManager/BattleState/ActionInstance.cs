using UnityEngine;
using System.Collections.Generic;

public class ActionInstance
{
    public SpeedSlot SourceSlot;
    public SpeedSlot TargetSlot;
    
    public CardData Card;

    public int Speed => SourceSlot.speed;

    public int RegisterOrder; // 높을수록 먼저 등록된 행동
}
