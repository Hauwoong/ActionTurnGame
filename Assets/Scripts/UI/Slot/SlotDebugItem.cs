using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotDebugItem : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI ownerText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI cardText;
    [SerializeField] private TextMeshProUGUI interceptText;
    [SerializeField] private TextMeshProUGUI boutText;
    [SerializeField] private Button button;
    [SerializeField] private Image bg;

    private PlayerActionInput _input;
    private SpeedSlot _slot;

    public void Bind(SpeedSlotRuntime slotRuntime, BoutGraph graph, PlayerActionInput input)
    {
        _slot = slotRuntime.Slot;
        _input = input;

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);

        ownerText.text = $"ID: {_slot.CharacterId}";
        speedText.text = $"SPD: {slotRuntime.Speed}";

        graph.ActionBySlot.TryGetValue(_slot, out var action);

        targetText.text = action != null
            ? $"{action.TargetSlot.CharacterId}-{action.TargetSlot.SlotIndex}"
            : "None";

        cardText.text = action?.Card != null
            ? action.Card.CardName
            : "No Card";

        int interceptCount = graph.interceptCandidates.TryGetValue(_slot, out var candidates)
            ? candidates.Count
            : 0;
        interceptText.text = interceptCount > 0 ? $"Intercept: {interceptCount}" : "Intercept: -";

        bool hasBout = graph.edges.TryGetValue(_slot, out var boutPartner);
        boutText.text = hasBout ? $"Bout: {boutPartner.CharacterId}-{boutPartner.SlotIndex}" : "Bout: -";

        UpdateColor(slotRuntime, hasBout, interceptCount);
    }

    private void UpdateColor(SpeedSlotRuntime slotRuntime, bool hasBout, int interceptCount)
    {
        if (bg == null) return;

        if (slotRuntime.Used)
            bg.color = Color.gray;
        else if (hasBout)
            bg.color = Color.red;
        else if (interceptCount > 0)
            bg.color = new Color(1f, 0.6f, 0f);
        else
            bg.color = Color.white;
    }

    private void OnClick()
    {
        _input.SelectSpeedSlot(_slot);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_input.IsDraggingCard()) return;

        _input.RegisterToSlot(_slot);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        _input.CancelSlot(_slot);
    }
}
