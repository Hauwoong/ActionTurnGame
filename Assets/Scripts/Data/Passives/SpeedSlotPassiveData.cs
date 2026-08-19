using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Speed Slot")]
public class SpeedSlotPassiveData : PassiveData
{
    public override PassiveModel ToModel() => new SpeedSlotModel();
}