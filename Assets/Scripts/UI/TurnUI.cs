using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private Button endTurnButton;

    [Header("Reference")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Player player;

    private void Start()
    {
        if (battleManager == null || player == null)
        {
            Debug.LogError("TurnUI: Missing TurnManager or Player reference.");
            return;
        }
    }

    public void onClickEndButton()
    {
        battleManager.EndTurn();
    }
}
