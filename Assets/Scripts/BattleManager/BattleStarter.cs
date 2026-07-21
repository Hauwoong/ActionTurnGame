using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    private void Start()
    {
        if (battleManager == null || player == null || enemy == null)
        {
            Debug.LogError("BattleStarter: Missing reference.");
            return;
        }

        battleManager.CreateBattle(player.SelectedParty, enemy.Members);
        battleManager.StartTurn();
    }
}
