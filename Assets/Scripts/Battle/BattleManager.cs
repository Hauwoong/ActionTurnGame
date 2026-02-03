
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

    List<Character> units = new();
    List<ActionSlot> speedQueue = new();

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

        RollSpeed();
        BuildSpeedQueue();
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
        player.RollspeedDice();
        enemy.RollspeedDice();
    }

    void BuildSpeedQueue()
    {
        speedQueue.Clear();

        foreach (var unit in units)
        {
            foreach (var v in unit.rolledSpeeds)
            {
                speedQueue.Add(new ActionSlot
                {
                    owner = unit,
                    //card = unit.selectedCard, // 따로 만들어야 함
                    speed = v
                });
            }
        }
    }

    void SortSpeed()
    {
        speedQueue.Sort((a, b) => b.speed.CompareTo(a.speed));
    }

    void ProcessTurn() // 이건 턴 진행 버튼 만들거임 
    {
        ResolveBattle();
    }

    void ResolveBattle()
    {
        foreach (var v in speedQueue)
        {
            
        }
    }

    void ResolveClash(Player P, Enemy e)
    {
        while (P.HasDice && e.HasDice)
        {
            var pd = P.PopDice();
            var ed = P.PopDice();

            ResolveDiceClash(pd, ed);
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

    void ApplyRemainingDice(Character c, Character target)
    {
        while (c.HasDice)
        {
            var d = c.PopDice();

            if (d.type == DiceType.Attack)
            {
                target.TakeDamage(d.value);
            }
        }
    }
}
