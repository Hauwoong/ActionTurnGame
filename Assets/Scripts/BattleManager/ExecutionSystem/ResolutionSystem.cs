using System.Collections.Generic;
using UnityEngine;

public class ResolutionSystem
{
    BattleState state;
    private readonly PriorityQueue<SpeedSlot, ActionPriority> executionQueue = new();
    private HashSet<SpeedSlot> resolved = new();

    public ResolutionSystem(BattleState state)
    {
        this.state = state;
    }

    public void Resolve(BattleState state)
    {
        executionQueue.Clear(); // Clear the execution queue at the start of each resolution phase
        resolved.Clear(); // Clear the resolved set at the start of each resolution phase

        BuildExecutionQueue(state);

        while (executionQueue.Count > 0)
        {
            SpeedSlot slot = executionQueue.Dequeue();

            if (resolved.Contains(slot))
            {
                continue;
            }

            if (state.BoutGraph.edges.TryGetValue(slot, out var opponent))
            {
                ResolveClash(slot, opponent);
                resolved.Add(slot);
                resolved.Add(opponent);
            }

            else
            {
                ResolveUnopposed(slot);
                resolved.Add(slot);
            }
        }
    }

    void BuildExecutionQueue(BattleState state)
    {
        foreach (var kvp in state.ActionBySlot)
        {
            var slot = kvp.Key;
            var action = kvp.Value;

            executionQueue.Enqueue(slot, new ActionPriority
            {
                Speed = slot.speed,
                RegisterOrder = action.RegisterOrder
            });
        }
    }
}
