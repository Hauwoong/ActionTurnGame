
using UnityEngine;
using System.Collections.Generic;

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
    private BattleContext ctx;

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

        ctx = new BattleContext(player, enemy);

        RollSpeed();
        SortSpeed();
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

    void RollSpeed()
    {
        ctx.speedResults.Clear();

        foreach (var v in player.rolledSpeeds)
        {
            ctx.speedResults.Add(new SpeedDiceResult()
            {
                owner = player,
                speed = v
            });
        }

        foreach (var v in enemy.rolledSpeeds)
        {
            ctx.speedResults.Add(new SpeedDiceResult()
            {
                owner = enemy,
                speed = v
            });
        }
    }

    void SortSpeed()
    {
        ctx.speedResults.Sort((a, b) => b.speed.CompareTo(a.speed));
    }

    void ProcessTurn() // 이건 턴 진행 버튼 만들거임 
    {
        foreach (var r in ctx.speedResults)
        {
            if (!r.IsReady) { continue; }

            r.card.Use(ctx);
        }

        ResolveBattle(ctx);
    }

    void ResolveBattle(BattleContext ctx)
    {
        for (int i = 0; i + 1 < ctx.allDice.Count; i += 2)
        {
            DiceResult a = ctx.allDice[i];
            DiceResult b = ctx.allDice[i + 1];

            ResolveDiceClash(a, b);
        }
    }

    // character 수정해야함 
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
