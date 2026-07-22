using UnityEngine;

public class PlayerActionInput : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    private SpeedSlot? selectedSlot = null;
    private CardModel draggingCard = null;

    public void SelectSpeedSlot(SpeedSlot slot)
    {
        selectedSlot = slot;
        Debug.Log($"Speed Dice {selectedSlot.Value.SlotIndex} selected");
    }

    public void StartDraggingCard(CardModel card)
    {
        Debug.Log("Drag Start");
        draggingCard = card;
    }

    public void RegisterToSlot(SpeedSlot targetSlot)
    {
        if (!selectedSlot.HasValue || draggingCard == null) return;

        var runtime = battleManager.Runtime;
        runtime.EnqueueEvent(new ActionRegisteredEvent(
            selectedSlot.Value, targetSlot, draggingCard
        ));

        draggingCard = null;
        selectedSlot = null;
    }

    public void CancelSlot(SpeedSlot slot)
    {
        var runtime = battleManager.Runtime;
        runtime.EnqueueEvent(new ActionCancelledEvent(slot));
    }

    public void EndDraggingCard()
    {
        draggingCard = null;
    }

    public bool HasSelectedSlot()
    {
        return selectedSlot.HasValue;
    }

    public bool IsDraggingCard()
    {
        return draggingCard != null;
    }
}
