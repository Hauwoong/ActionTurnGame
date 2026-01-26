using TMPro;
using UnityEngine;

public class HPTextUI : MonoBehaviour
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
        UpdateHPUI(player.currentHP,player.MaxHP);
    }

    private void UpdateHPUI(int CurrentHP, int MaxHP)
    {
        hpText.text = $"HP: {CurrentHP}/{MaxHP}";
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnHPChanged -= UpdateHPUI;
    }
}
