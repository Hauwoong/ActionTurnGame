
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

    private List<Character> units = new();
    public IReadOnlyList<Character> Units => units;
    List<SpeedSlot> allSlots = new();

    private BattleState state = BattleState.Ready;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    [SerializeField] SlotDebugPanel debugPanel;

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
        RollSpeed();

        allSlots.Clear();

        foreach (var unit in units)
        {
            unit.OnTurnStart();
            allSlots.AddRange(unit.speedSlots);
        }

        debugPanel.Refresh();
    }

    public void EndTurn()
    {
        ResolveTurn();
    }

    public void ResolveTurn()
    {
        BuildInterceptMap();

        ResolveBattle();

        foreach (var unit in units)
            unit.ClearDiceStack();
        
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

        mySlot.card = card;
        mySlot.target = targetSlot;
        mySlot.Use();

        Debug.Log($"{mySlot.owner.name} registered {card.cardName}");

        debugPanel.Refresh();
    }

    void BuildInterceptMap()
    {
        foreach (var slot in allSlots)
        {
            slot.interceptCandidates.Clear();
            slot.currentBout = null;
        }

        foreach (var a in allSlots)
        {
            if (a.target == null) continue;

            foreach (var b in allSlots)
            {
                if (a == b) continue;
                if (b.target == null) continue;

                if (a.target == b.target)
                {
                    if (b.speed > a.target.speed)
                    {
                        a.interceptCandidates.Add(b);
                    }
                }
            }
        }
        debugPanel.Refresh();
    }

    void ResolveBattle()
    {
        var ordered = allSlots.OrderByDescending(s => s.speed).ToList();

        foreach (var slot in ordered)
        {
            if (slot.card == null) continue;

            slot.card.Use(new BattleContext(slot.owner, slot.target.owner));

            if (slot.currentBout != null)
            {
                ResolveClash(slot.owner, slot.currentBout.owner);
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
