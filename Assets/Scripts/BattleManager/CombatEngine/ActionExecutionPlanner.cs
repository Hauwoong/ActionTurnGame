
using System.Collections;

public class ActionExecutionPlanner
{
    public PriorityQueue<ActionInstance, ActionPriority> BuildQueue(BoutGraph graph)
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
}
