using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    [Header("Reference")]
    [SerializeField] private Player player;

    private void Start()
    {
        if (player == null || hpSlider == null)
        {
            Debug.LogError("HPUI or hpSlider: Missing Player reference.");
            return;
        }
        player.OnHPChanged += UpdateHPBar;
        UpdateHPBar(player.currentHP, player.MaxHP);
    }

    private void UpdateHPBar(int CurrentHP, int MaxHP)
    {
        hpSlider.maxValue = MaxHP;
        hpSlider.value = CurrentHP;
    }
}
