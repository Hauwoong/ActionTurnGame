using System;
using System.Collections.Generic;

public class BoutPlanner
{
    private readonly Dictionary<SpeedSlot, ActionDeclaration> actions = new();

    private readonly Dictionary<SpeedSlot, List<SpeedSlot>> targetMap = new();

    private readonly BoutGraph graph;

    private readonly Func<SpeedSlot, int> speedResolver;

    public BoutPlanner(BoutGraph graph, Func<SpeedSlot, int> speedResolver)
    {
        this.graph = graph;
        this.speedResolver = speedResolver;
    }

    public void Register(ActionDeclaration action)
    {
        actions[action.Slot] = action;

        AddToTargetMap(action);

        UpdateRealationsFor(action);
    }

    void AddToTargetMap(ActionDeclaration action)
    {
        SpeedSlot target = action.Target;

        if (!targetMap.ContainsKey(target))
        {
            targetMap[target] = new List<SpeedSlot>();
        }

        targetMap[target].Add(action.Slot);
    }

    void TryBuildDirectClash(ActionDeclaration action)
    {
        var source = action.Slot;
        var target = action.Target;

        if (!actions.ContainsKey(source)) return;

        var counter = actions[target];

        if (counter.Target.Equals(source))
        {
            graph.Connect(source, target);
        }
    }

    void TryBuildIntercept(ActionDeclaration action)
    {
        var source = action.Slot;
        var target = action.Target;

        if (!actions.ContainsKey(source)) return;

        int attackerSpeed = speedResolver(source);
        int targetSpeed = speedResolver(target);

        if (attackerSpeed > targetSpeed)
        {
            if (!graph.IsConnected(target))
            {
                graph.Connect(source, target);
            }
        }
    }
}
