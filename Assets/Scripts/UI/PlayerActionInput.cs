using NUnit.Framework.Constraints;
using UnityEngine;

public class PlayerActionInput : MonoBehaviour
{
    public Player player;
    public BattleManager battleManager;

    SpeedSlot selectedSlot = null;
    CardData draggingCard = null;

    public void SelectSpeedSlot(SpeedSlot slot)
    {
        selectedSlot = slot;

        Debug.Log($"Speed Dice {selectedSlot.index} selected");
    }

    public void StartDraggingCard(CardData card)
    {
        draggingCard = card;
    }

    public void RegisterToSlot(SpeedSlot targetSlot)
    {
        if (selectedSlot != null) return;

        battleManager.RegisterAction(selectedSlot, targetSlot, draggingCard);

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

    public bool IsDraggingCard()
    {
        return draggingCard != null;
    }
}
