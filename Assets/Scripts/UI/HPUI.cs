using TMPro;
using UnityEngine;

public class HPUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text hpText;

    [Header("Reference")]
    [SerializeField] private Player player;

    private void Start()
    {
        if (player == null || hpText == null)
        {
            Debug.LogError("HPUI or hpText: Missing Player reference.");
            return;
        }
        player.OnHPChanged += UpdateHPUI;
        UpdateHPUI(player.currentHP);
    }

    private void UpdateHPUI(int CurrentHP)
    {
        hpText.text = $"HP: {CurrentHP}/{player.MaxHP}";
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnHPChanged -= UpdateHPUI;
    }
}
