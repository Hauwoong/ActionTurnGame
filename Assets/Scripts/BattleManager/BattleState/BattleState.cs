using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class BattleState
{
    private List<Character> units = new();
    public IReadOnlyCollection<Character> Units => units;

    private Dictionary<SpeedSlot, ActionInstance> actionBySlot = new();
    public IReadOnlyDictionary<SpeedSlot, ActionInstance> ActionBySlot => actionBySlot;

    public BoutGraph BoutGraph;

    public List<CombatLog> CombatLogs = new();

    public LorRuleSet Rules;

    public IRng Rng { get; private set; }

    public BattleState(int seed)
    {
        Rng = new DeterministicRng(seed);
        BoutGraph = new BoutGraph(this);
    }

    public void StartNewTurn()
    {
        Clear();
        RollSpeedDice();
        InitializeRuntime();
    }

    void RollSpeedDice()
    {
        foreach (var unit in Units)
        {
            unit.RollspeedDice();
        }
    }
    
    public void RegisterAction(ActionInstance action)
    {
        if (actionBySlot.ContainsKey(action.SourceSlot))
        {
            Debug.LogError("Slot already has action.");
            return;
        }

        actionBySlot[action.SourceSlot] = action;
        BoutGraph.RegisterAction(action);
    }

    public void CancelAction(SpeedSlot slot)
    {
        if (!actionBySlot.TryGetValue(slot, out var action))
        {
            Debug.LogError("Slot does not have action.");
            return;
        }

        actionBySlot.Remove(slot);

        BoutGraph.CancelAction(action);
    }
    
    void InitializeRuntime()
    {
       
    }

    public void Clear()
    {
        actionBySlot.Clear();
        BoutGraph?.Clear();
        CombatLogs?.Clear();
    }
}
