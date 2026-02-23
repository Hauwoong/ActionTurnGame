using System.Collections.Generic;

public class ResolutionSystem
{
    BattleState state;
    private readonly PriorityQueue<SpeedSlot, ActionPriority> executionQueue = new();
    private HashSet<SpeedSlot> resolved = new();

    public ResolutionSystem(BattleState state)
    {
        this.state = state;
    }

    public void Resolve()
    {
        executionQueue.Clear(); // Clear the execution queue at the start of each resolution phase
        resolved.Clear(); // Clear the resolved set at the start of each resolution phase

        BuildExecutionQueue();

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

    void BuildExecutionQueue()
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

    void ResolveClash(SpeedSlot slot, SpeedSlot opponent)
    {
        ActionRuntime aRuntime = CreateRuntime(state.ActionBySlot[slot]);
        ActionRuntime bRuntime = CreateRuntime(state.ActionBySlot[opponent]);

        CombatLog log = CombatEngine.ClashResolve(aRuntime, bRuntime);
        
        state.CombatLogs.Add(log);
    }

    void ResolveUnopposed(SpeedSlot slot)
    {

    }

    ActionRuntime CreateRuntime(ActionInstance action)
    {
        ActionRuntime runtime = new ActionRuntime
        {
            Source = action,
            DicePool = new List<DiceRuntime>(),
            CurrentIndex = 0,
        };

        foreach (var dice in action.Card.dices)
        {
            runtime.DicePool.Add(new DiceRuntime
            {
                Type = dice.type,
                Min = dice.min,
                Max = dice.max,
                IsDestoryed = false
            });
        }

        return runtime;
    }
}
