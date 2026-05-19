using UnityEngine;

public class PlayerActionInput : MonoBehaviour
{
    public BattleManager battleManager;

    private SpeedSlot? selectedSlot = null;
    [SerializeField] CardData draggingCard = null;

    public void SelectSpeedSlot(SpeedSlot slot)
    {
        selectedSlot = slot;

        Debug.Log($"Speed Dice {selectedSlot.Value.SlotIndex} selected");
    }

    public void StartDraggingCard(CardData card)
    {
        Debug.Log("Drag Start");
        draggingCard = card;
    }

    public void RegisterToSlot(SpeedSlot targetSlot)
    {
        if (selectedSlot == null) return;

        battleManager.RegisterAction(selectedSlot.Value, targetSlot, draggingCard);

        draggingCard = null;
        selectedSlot = null;
    }

    public void EndDraggingCard()
    {
        draggingCard = null;
    }

    public bool HasSelectedSlot()
    {
        return selectedSlot != null; 
    }

    public bool ISDraggingCard()
    {
        return draggingCard != null;
    }
}
