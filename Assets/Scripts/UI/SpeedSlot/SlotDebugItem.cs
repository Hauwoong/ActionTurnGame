using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotDebugItem : MonoBehaviour
{
    public TextMeshProUGUI OwnerText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI cardText;
    public TextMeshProUGUI interceptText;
    public TextMeshProUGUI boutText;

    public Image bg;

    public void Bind(SpeedSlot slot)
    {
        OwnerText.text = slot.owner.Name;
        speedText.text = $"SPD: {slot.speed}";

        targetText.text =
            slot.target != null
            ? $"{slot.target.owner.Name}"
            : "None";

        cardText.text =
            slot.card != null
            ? $"{slot.card.cardName}"
            : "No Card";

        interceptText.text = 
            slot.interceptCandidates.Count > 0
            ? $"Intercept: {slot.interceptCandidates.Count}"
            : "Intercept: -";

        boutText.text =
            slot.currentBout != null
            ? $"Bout: {slot.currentBout.owner.Name}"
            : "Bout: -";

        UpdateColor(slot);
    }

    void UpdateColor(SpeedSlot slot)
    {
        if (bg == null) return;

        if (slot.IsUsed)
            bg.color = Color.gray;
        else if (slot.currentBout != null)
            bg.color = Color.red;
        else if (slot.interceptCandidates.Count > 0)
            bg.color = new Color(1f, 0.6f, 0f);
        else
            bg.color = Color.white;
    }
}
