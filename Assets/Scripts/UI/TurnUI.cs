using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Button endTurnButton;

    [Header("Reference")]
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Player player;

    private void Start()
    {
        if (turnManager == null || player == null)
        {
            Debug.LogError("TurnUI: Missing TurnManager or Player reference.");
            return;
        }

        // onTurnChanged 이벤트에 UpdateUI 메서드 등록
        turnManager.OnTurnChanged += UpdateUI;

        endTurnButton.onClick.AddListener(onClickEndButton);

        UpdateUI(turnManager.State);
    }

    private void UpdateUI(TurnManager.TurnState state) //이벤트가 state를 인자로 넘겨줌
    {
        if (state == TurnManager.TurnState.PlayerTurn)
        {
            turnText.text = "Player's Turn";
            endTurnButton.interactable = true;
        }

        else
        {
            turnText.text = "Enemy's Turn";
            endTurnButton.interactable = false;
        }

        energyText.text = $"Energy: {player.currentEnergy}/{player.MaxEnergy}";
    }

    public void onClickEndButton()
    {
        turnManager.PlayerEndTurn();
    }
}
