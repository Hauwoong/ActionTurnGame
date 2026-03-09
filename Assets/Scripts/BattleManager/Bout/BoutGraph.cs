using System.Collections.Generic;
public class BoutGraph
{
    private IReadOnlyDictionary<SpeedSlot, ActionInstance> actionBySlot => state.ActionBySlot;

    //타겟 => 그 타겟을 노리는 모든 행동
    private Dictionary<SpeedSlot, List<ActionInstance>> targetMap = new();

    // 실제 확정된 합들
    public Dictionary<SpeedSlot, SpeedSlot> edges = new();

    // 합 후보들
    public Dictionary<SpeedSlot, List<SpeedSlot>> interceptCandidates = new();

    public void RegisterAction(ActionInstance action)
    {
        AddToTargetMap(action);

        UpdateRelationsFor(action);
    }

    public void CancelAction(ActionInstance action)
    {
        RemoveFromTargetMap(action);

        RemoveEdgesInvolving(action);

        RemoveInterceptCandidate(action);

        ReevaluateAffectedSlots(action);
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

    void RemoveFromTargetMap(ActionInstance action)
    {
        var targetSlot = action.TargetSlot;

        if (targetMap.TryGetValue(targetSlot, out var list))
        {
            list.Remove(action);

            if (list.Count == 0)
            {
                targetMap.Remove(targetSlot);
            }
        }
    }

    void UpdateRelationsFor(ActionInstance action)
    {
        // 인터셉트 검사
        TryBuildInterceptClash(action);

        // 정명 합 검사
        TryBuildDirectClash(action);
    }
    void TryBuildInterceptClash(ActionInstance action)
    {
        var source = action.SourceSlot;
        var target = action.TargetSlot;

        var targetAction = actionBySlot.ContainsKey(target) ? actionBySlot[target] : null;
        if (targetAction == null) return;

        if (source.speed > targetAction.SourceSlot.speed)
        {
            AddInterceptCandidate(target, source);

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

    void TryBuildDirectClash(ActionInstance action)
    {
        var Source = action.SourceSlot;
        var target = action.TargetSlot;

        if (edges.ContainsKey(target)) return; // 이미 타겟이 다른 행동과 합 되어있음 => 이미 intercetp 당한 상태

        var counter = actionBySlot[target];

        if (counter.TargetSlot == action.SourceSlot)
        {
            Connect(Source, target);
        }
    }

    void RemoveEdgesInvolving(ActionInstance action)
    {
        SpeedSlot Source = action.SourceSlot;

        if (!edges.ContainsKey(Source))
        {
            return;
        }

        Disconnect(Source);
    }

    void AddInterceptCandidate(SpeedSlot target, SpeedSlot attacker)
    {
        if (!interceptCandidates.ContainsKey(target))
        {
            interceptCandidates[target] = new List<SpeedSlot>();
        }

        if (!interceptCandidates[target].Contains(attacker))
        {
            interceptCandidates[target].Add(attacker);
        }
    }

    void RemoveInterceptCandidate(ActionInstance action)
    {
        var target = action.TargetSlot;
        var source = action.SourceSlot;

        if (!interceptCandidates.ContainsKey(target)) return;

        if (!interceptCandidates[target].Contains(source)) return;

        interceptCandidates[target].Remove(source);
    }

    public void Connect(SpeedSlot a, SpeedSlot b)
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

    void ReevaluateAffectedSlots(ActionInstance cancelled)
    {
        SpeedSlot affectedTarget = cancelled.TargetSlot;

        if (targetMap.TryGetValue(affectedTarget, out List<ActionInstance> attackers))
        {
            foreach (var attacker in attackers)
            {
                UpdateRelationsFor(attacker);
            }
        }
    }

    public void Clear()
    {
        targetMap.Clear();
        edges.Clear();
        interceptCandidates.Clear();
    }
}
