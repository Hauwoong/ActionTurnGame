using System.Collections.Generic;
public class BoutGraph
{
    private Dictionary<SpeedSlot, ActionInstance> actionBySlot;
    public Dictionary<SpeedSlot, ActionInstance> ActionBySlot => actionBySlot;

    private readonly ISlotLookup slotLookup;

    private Dictionary<SpeedSlot, List<ActionInstance>> targetMap = new();

    public Dictionary<SpeedSlot, SpeedSlot> edges = new();

    public Dictionary<SpeedSlot, List<SpeedSlot>> interceptCandidates = new();

    public BoutGraph(Dictionary<SpeedSlot, ActionInstance> actionBySlot, ISlotLookup slotLookup)
    {
        this.actionBySlot = actionBySlot;
        this.slotLookup = slotLookup;
    }

    public void RegisterAction(ActionInstance action)
    {
        if (actionBySlot.TryGetValue(action.SourceSlot, out var current))
            CancelAction(current);

        actionBySlot[action.SourceSlot] = action;

        AddToTargetMap(action);

        UpdateRelationsFor(action);
    }

    public void CancelAction(ActionInstance action)
    {
        RemoveFromActionBySlot(action);

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

    void RemoveFromActionBySlot(ActionInstance action)
    {
        if (actionBySlot.TryGetValue(action.SourceSlot, out var current) && ReferenceEquals(current, action))
            actionBySlot.Remove(action.SourceSlot);
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
        // ���ͼ�Ʈ �˻�
        TryBuildInterceptClash(action);

        // ���� �� �˻�
        TryBuildDirectClash(action);
    }
    void TryBuildInterceptClash(ActionInstance action)
    {
        var source = action.SourceSlot;
        var target = action.TargetSlot;

        if (!actionBySlot.ContainsKey(target)) return;

        var sourceSpeed = slotLookup.GetSlotRuntime(source).Speed;
        var targetSpeed = slotLookup.GetSlotRuntime(target).Speed;

        if (sourceSpeed > targetSpeed)
        {
            AddInterceptCandidate(target, source);

            if (edges.ContainsKey(target))
            {
                var current = edges[target];

                if (!actionBySlot.TryGetValue(current, out var currentAction)) return;

                if (action.ActionId > currentAction.ActionId)
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

        if (edges.ContainsKey(target)) return; // �̹� Ÿ���� �ٸ� �ൿ�� �� �Ǿ����� => �̹� intercetp ���� ����

        if (!actionBySlot.TryGetValue(target, out var counter)) return;

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
        if (!edges.TryGetValue(a, out var other))
            return;

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
        actionBySlot.Clear();
        targetMap.Clear();
        edges.Clear();
        interceptCandidates.Clear();
    }
}
