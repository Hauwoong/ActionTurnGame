using System.Collections.Generic;
using UnityEngine;

public class ExecutionSystem
{
    public void Resolve(BattleState state)
    {
        var queue = BuildPriorityQueue(state);
        var resolved = new HashSet<ActionInstance>();
        var actionMap = BuildActionMap(state);
        var Bouts = new List<Bout>();

        while (queue.Count > 0)
        {
            ActionInstance action = queue.Dequeue();

            if (resolved.Contains(action)) continue;    

            if (TryFindBout(action, actionMap, out var opponent))
            {
                ResolveClash(action, opponent);
                resolved.Add(action);
                resolved.Add(opponent);
            }

            else
            {
                ResolveUnopposed(action);
                resolved.Add(action);
            }

                ExecuteAction(action);
        }
    }

    public PriorityQueue<ActionInstance, int> BuildPriorityQueue(BattleState state)
    {
        var pq = new PriorityQueue<ActionInstance, int>();
        foreach (var action in state.RegisterActions)
        {
            pq.Enqueue(action, action.Speed);
        }
        return pq;
    }

    Dictionary<Character, ActionInstance> BuildActionMap(BattleState state)
    {
        var map = new Dictionary<Character, ActionInstance>();

        foreach (var action in state.RegisterActions)
        {
            map[action.SourceSlot.owner] = action;
        }

        return map;
    }

    bool TryFindBout(
        ActionInstance action,
        Dictionary<Character, ActionInstance> map,
        out ActionInstance opponent)
    {
        opponent = null;

        var attacker = action.SourceSlot.owner;
        var defender = action.TargetSlot.owner;

        if (!map.ContainsKey(defender))
            return false;

        var counterAction = map[defender];
        
        if (counterAction.TargetSlot.owner == attacker)
        {
            opponent = counterAction;
            return true;
        }

        return false;
    }

    void BuildIntercepts(
        BattleState state,
        Dictionary<Character, ActionInstance> actionMap,
        List<Bout> bouts)
    {
        var targetMap = BuildTargetMap(state);

        foreach (var target in targetMap.Keys)
        {
            if (!actionMap.ContainsKey(target)) continue;

            var targetAction = actionMap[target];

            foreach (var attacker in targetMap[target])
            {
                if (attacker == targetAction) continue;

                if (attacker.TargetSlot.owner == targetAction.SourceSlot.owner) continue;

                if (attacker.Speed > targetAction.Speed)
                {
                    bouts.Add(new Bout(attacker, targetAction));
                }
            }
        }
    }

    Dictionary<Character, List<ActionInstance>> BuildTargetMap(BattleState state)
    {
        var map = new Dictionary<Character, List<ActionInstance>>();

        foreach (var action in state.RegisterActions)
        {
            var target = action.TargetSlot.owner;

            if (!map.ContainsKey(target))
            {
                map[target] = new List<ActionInstance>();
            }
            map[target].Add(action);
        }
        return map;
    }

    void ExecuteAction(ActionInstance action)
    {
        var attacker = action.SourceSlot.owner;
        var defender = action.TargetSlot.owner;

        action.Card.Use(new BattleContext(attacker, defender));

        var opponentAction = FindOpposingAction(action);

        if (opponentAction != null)
        {
            ResolveClash(action, opponentAction);
        }

        else
        {
            ResolveUnopposed(action);
        }
    }

    ActionInstance FindOpposingAction(ActionInstance action)
    {
        // TODO: 나중에 BoutSystem으로 분리 가능
        return null;
    }

    void ResolveClash(ActionInstance a, ActionInstance b)
    {
        var p = a.SourceSlot.owner;
        var e = b.SourceSlot.owner;

        while (p.HasDice && e.HasDice)
        {
            var pd = p.PopDice();
            var ed = e.PopDice();

            ClashResolver.Resolve(pd, ed);
        }
    }

    void ResolveUnopposed(ActionInstance action)
    {
        var attacker = action.SourceSlot.owner;
        var defender = action.TargetSlot.owner;

        while (attacker.HasDice)
        {
            var pd = attacker.PopDice();
            if (pd.type == DiceType.Attack)
            {
                defender.TakeDamage(pd.value);
            }
        }
    }
}
