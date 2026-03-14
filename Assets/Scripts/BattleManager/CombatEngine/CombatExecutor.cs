using System.Collections.Generic;

public class CombatExecutor
{
    private readonly BattleRuntime runtime;
    private readonly IRuleSet rules;
    private readonly IRng rng;

    public CombatExecutor(IRuleSet rules, IRng rng, BattleRuntime runtime)
    {
        this.rules = rules;
        this.rng = rng;
        this.runtime = runtime;
    }
  
    public void Execute(BoutGraph graph)
    {
        var queue = BuildQueue(graph);

        RunQueue(queue,graph);
    }
    PriorityQueue<ActionInstance, ActionPriority> BuildQueue(BoutGraph graph)
    {
        var pq = new PriorityQueue<ActionInstance, ActionPriority>();

        foreach (var action in graph.ActionBySlot.Values)
        {
            var slot = action.SourceSlot;

            var priority = new ActionPriority
            {
                Speed = graph.SlotRuntime[slot].Speed,
                RegisterOrder = action.RegisterOrder
            };

            pq.Enqueue(action, priority);
        }

        return pq;
    }

    void RunQueue(PriorityQueue<ActionInstance, ActionPriority> queue, BoutGraph graph)
    {
        var visited = new HashSet<SpeedSlot>();

        while (queue.Count > 0)
        {
            var action = queue.Dequeue();
            var slot = action.SourceSlot;

            if (visited.Contains(slot)) continue;

            var targetSlot = action.TargetSlot;

            if (graph.ActionBySlot.TryGetValue(targetSlot, out var opponent))
            {
                if (visited.Contains(targetSlot)) continue;

                ResolveClash(action, opponent);

                visited.Add(slot);
                visited.Add(targetSlot);
            }

            else
            {
                ResolveUnopposed(action);

                visited.Add(slot);
            }
        }
    }
}
