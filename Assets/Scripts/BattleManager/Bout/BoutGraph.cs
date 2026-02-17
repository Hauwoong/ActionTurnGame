using System.Collections.Generic;
public class BoutGraph
{
    // 모든 행동들
    public List<ActionInstance> Actions = new();

    // Character => action 현재 행동
    private Dictionary<SpeedSlot, ActionInstance> actionBySlot = new();

    //타겟 => 그 타겟을 노리는 모든 행동
    private Dictionary<SpeedSlot, List<ActionInstance>> targetMap = new();

    // 실제 확정된 합들
    public Dictionary<SpeedSlot, SpeedSlot> edges = new();

    // 합 후보들
    public Dictionary<SpeedSlot, List<ActionInstance>> interceptCandidates = new();

    public void RegisterAction(ActionInstance action)
    {
        Actions.Add(action);

        actionBySlot[action.SourceSlot] = action;

        AddToTargetMap(action);

        UpdateRelationsFor(action);
    }

    void AddToTargetMap(ActionInstance action)
    {
        var targetSlot = action.TargetSlot;

        if (!targetMap.ContainsKey(targetSlot))
        {
            targetMap[targetSlot] = new List<ActionInstance>();
        }

        targetMap[targetSlot].Add(action);
    }

    void UpdateRelationsFor(ActionInstance action)
    {

        // 정면 합 검사
        TryBuildDirectClash(action);

        // 인터셉트 검사
        TryBuildInterceptClash(action);
    }

    void TryBuildDirectClash(ActionInstance action)
    {
        var Source = action.SourceSlot;
        var target = action.TargetSlot;

        if (!actionBySlot.ContainsKey(target))
        {
            return;
        }

        var counter = actionBySlot[target];
        
        if (counter.TargetSlot == action.SourceSlot)
        {
            Connect(Source,target);
        }
    }

    void TryBuildInterceptClash(ActionInstance action)
    {
        var source = action.SourceSlot;
        var target = action.TargetSlot;

        var targetAction = actionBySlot.ContainsKey(target) ? actionBySlot[target] : null;
        if (targetAction == null) return;

        if (source.speed > targetAction.SourceSlot.speed)
        {
            if (edges.ContainsKey(target))
            {
                var current = edges[target];

                var currentAction = actionBySlot[current];

                if (action.RegisterOrder > currentAction.RegisterOrder)
                {
                    Disconnect(target);
                    Connect(source, target);
                }
            }

            else
            {
                Connect(source, target);           
            }
            
        }
    }

    void Connect(SpeedSlot a, SpeedSlot b)
    {
        Disconnect(a);
        Disconnect(b);

        edges[a] = b;
        edges[b] = a;
    }

    void Disconnect(SpeedSlot a)
    {
        if (!edges.ContainsKey(a)) return;

        var other = edges[a];

        edges.Remove(a);
        edges.Remove(other);
    }
}
