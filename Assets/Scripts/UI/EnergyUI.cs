using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text energyText;

    [Header("Reference")]
    [SerializeField] private Player player;

    private void Start()
    {
        if (player == null || energyText == null)
        {
            Debug.LogError("EnergyUI: Missing Player reference.");
            return;
        }
        player.OnEnergyChanged += UpdateEnergyUI;
        UpdateEnergyUI(player.currentEnergy, player.MaxEnergy);
    }

    private void UpdateEnergyUI(int CurrentEnergy, int MaxEnergy)
    {
        energyText.text = $"Energy: {CurrentEnergy}/{MaxEnergy}";
    }
    private void OnDestroy()
    {
        if (player != null)
            player.OnEnergyChanged -= UpdateEnergyUI;
    }
}
