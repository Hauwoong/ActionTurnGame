using System.Runtime.Serialization;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private enum BattleState
    {
        Ready,
        Fighting,
        Win,
        Lose
    }

    private BattleState state = BattleState.Ready;

    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private TurnManager turnManager;

    public Enemy CurrentEnemy => enemy;

    public void Awake()
    {
        if (player == null || enemy == null || turnManager == null)
        {
            Debug.LogError("BattleManager: One or more required components are not assigned.");
            enabled = false;
            return;
        }
        StartBattle();
    }
    public void StartBattle()
    {
        if (state != BattleState.Ready)
        {
            Debug.LogWarning("BattleManager: Battle has already started or ended.");
            return;
        }
        state = BattleState.Fighting;
        turnManager.enabled = true;
        Debug.Log("BattleManager: Battle has started!");
    }

    public void EndBattle(bool playerWon)
    {
        if (state != BattleState.Win && state != BattleState.Lose)
        {
            if (playerWon)
            {
                state = BattleState.Win;
                Debug.Log("BattleManager: Player has won the battle!");
            }
            else
            {
                state = BattleState.Lose;
                Debug.Log("BattleManager: Player has lost the battle.");

            }
            
            turnManager.enabled = false;
        }
    }

    public void UseCard(CardData card)
    {
        if (state != BattleState.Fighting)
        {
            Debug.LogWarning("BattleManager: Cannot use cards when not in fighting state.");
            return;
        }

        if (card == null) return;

        BattleContext ctx = new BattleContext
        {
            user = player,
            target = enemy,
        };

        card.Use(ctx);

        player.RemoveCard(card);
    }
    
}
