
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
    List<ActionSlot> plannedActions = new();
    List<ActionSlot> speedQueue = new();

    private BattleState state = BattleState.Ready;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    public void Awake()
    {
        if (player == null || enemy == null)
        {
            Debug.LogError("BattleManager: One or more required components are not assigned.");
            enabled = false;
            return;
        }

        units.Add(player);
        units.Add(enemy);

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

        Debug.Log("BattleManager: Battle has started!");
    }
    public void StartTurn()
    {
        plannedActions.Clear();

        RollSpeed();

        foreach (var unit in units)
        {
            unit.OnTurnStart();
        }

        StartBattle();
    }

    public void EndTurn()
    {
        ResolveTurn();
    }

    public void ResolveTurn() // 이건 턴 진행 버튼 만들거임 
    {
        speedQueue.Clear();
        speedQueue.AddRange(plannedActions);

        SortSpeed();

        ResolveBattle();

        foreach (var unit in units)
        {
            unit.ClearDiceStack();
        }

        StartTurn();
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
        }
    }

    void RollSpeed()
    {
        player.RollspeedDice();
        enemy.RollspeedDice();
    }

    public void RegisterAction(Character owner, Character target, CardData card, int index)
    {   
        if (owner.IsDiceUsed(index))
        {
            Debug.Log("This dice already used!");
            return;
        }

        int speed = owner.rolledSpeeds[index];

        ActionSlot slot = new ActionSlot
        {
            owner = owner,
            target = target,
            card = card,
            speed = speed,
            diceIndex = index
        };

        plannedActions.Add(slot);

        owner.UseDice(index);

        Debug.Log($"Action Registered: {owner.name} / {card.cardName}");
    }

    void SortSpeed()
    {
        speedQueue.Sort((a, b) => b.speed.CompareTo(a.speed));
    }

    void ResolveBattle()
    {
        foreach (var action in speedQueue)
        {
            BattleContext ctx = new BattleContext(action.owner, action.target);
            action.card.Use(ctx);
            // 적이 사용한 카드 
            ResolveClash(ctx.currentActor, ctx.target);
        }
    }

    void ResolveClash(Character p, Character e)
    {
        while (p.HasDice && e.HasDice)
        {
            var pd = p.PopDice();
            var ed = e.PopDice();

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
