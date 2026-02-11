using UnityEngine;

public class SpeedSlotUI : MonoBehaviour
{
    public SpeedSlot slot;
    public PlayerActionInput input;

    public void Init(SpeedSlot slot, PlayerActionInput input)
    {
        this.slot = slot;
        this.input = input;
    }

    public void OnClick()
    {
        input.SelectSpeedSlot(slot);
    }
}
