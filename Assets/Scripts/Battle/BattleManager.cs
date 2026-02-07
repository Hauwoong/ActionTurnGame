
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        StartTurn();
    }
    public void StartTurn()
    {
        plannedActions.Clear();

        RollSpeed();

        foreach (var unit in units)
        {
            unit.OnTurnStart();
        }

    }

    public void EndTurn()
    {
        ResolveTurn();
    }

    public void ResolveTurn() // 이건 턴 진행 버튼 만들거임 
    {
        speedQueue.Clear();
        speedQueue.AddRange(plannedActions);

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

    public void RegisterAction(SpeedSlot mySlot, SpeedSlot targetSlot, CardData card)
    {
        if (mySlot.IsUsed)
        {
            Debug.Log("This slot already used!");
            return;
        }

        ActionSlot newSlot = new ActionSlot
        {
            mySlot = mySlot,
            targetSlot = targetSlot,
            card = card,
            isClashing = false,
            boutTarget = null
        };

        foreach (var old in plannedActions)
        {
            if (IsMutualTarget(newSlot, old)) // 서로 타겟팅일 경우
            {
                MakeBout(newSlot, old);
            }

            else if (CanIntercept(newSlot, old)) // 속도가 더 빠를 경우
            {
                MakeBout(newSlot, old);
            }
        }

        plannedActions.Add(newSlot);
        mySlot.Use();
    }

    bool IsMutualTarget(ActionSlot a, ActionSlot b)
    {
        return
            a.targetSlot == b.mySlot &&
            b.targetSlot == a.mySlot;
    }

    bool CanIntercept(ActionSlot a, ActionSlot b)
    {
        return
            a.targetSlot == b.targetSlot &&
            a.mySlot.speed > a.targetSlot.speed;
    }

    void MakeBout(ActionSlot a, ActionSlot b)
    {
        a.isClashing = true;
        b.isClashing = true;

        a.boutTarget = b;
        b.boutTarget = a;
    }

    void ResolveBattle()
    {
        var ordered = plannedActions.OrderByDescending(s => s.mySlot.speed).ToList();

        foreach (var slot in ordered)
        {
            if (slot.card == null) continue;

            slot.card.Use(new BattleContext(slot.mySlot.owner, slot.targetSlot.owner));

            if (slot.isClashing)
            {
                ResolveClash(slot.mySlot.owner, slot.boutTarget.mySlot.owner);
            }
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
