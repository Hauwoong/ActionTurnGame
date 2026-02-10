using UnityEngine;

public class SpeedSlotUI : MonoBehaviour
{
    public SpeedSlot slot;
    public PlayerActionInput input;

    public void onClick()
    {
        input.SelectSpeedSlot(slot);
    }
}
