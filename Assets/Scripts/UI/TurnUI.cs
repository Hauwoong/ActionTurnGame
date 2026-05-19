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

    private void Start()
    {
        if (battleManager == null)
        {
            Debug.LogError("TurnUI: Missing BattleManager reference.");
            return;
        }
    }

    public void onClickEndButton()
    {
        battleManager.EndTurn();
    }
}
