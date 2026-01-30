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

        var ctx = new BattleContext(player, enemy);

        bool success = card.Use(ctx); // 아마 수정 해야 함

        // 적 카드 사용 굴림

        if (success)
        {
            ResolveBattle(ctx);
            player.RemoveCard(card);
        }
    }

    void ResolveBattle(BattleContext ctx)
    {
        int count = Mathf.Min(
            ctx.playerDice.Count,
            ctx.enemyDice.Count
        );

        for (int i = 0; i < count; i++)
        {
            ResolveDiceClash(ctx.playerDice[i], ctx.enemyDice[i]);
        }
    }

    void ResolveDiceClash(DiceResult P, DiceResult E)
    {
        if (P.type == DiceType.Attack && E.type == DiceType.Attack)
        {
            if (P.value > E.value)
            {
                E.owner.TakeDamage(P.value);
            }
            else if (P.value < E.value)
            {
                P.owner.TakeDamage(E.value);
            }
            else
            {
                Debug.Log("Two Player Draw!");
            }
        }

        else if (P.type == DiceType.Attack && E.type == DiceType.Block)
        {
            int dmg = P.value - E.value;

            if (dmg > 0)
            {
                E.owner.TakeDamage(dmg);
            }
        }

        else if (E.type == DiceType.Attack && P.type == DiceType.Block)
        {
            int dmg = E.value - P.value;

            if (dmg > 0)
            {
                P.owner.TakeDamage(dmg);
            }
        }
    }
    
}
