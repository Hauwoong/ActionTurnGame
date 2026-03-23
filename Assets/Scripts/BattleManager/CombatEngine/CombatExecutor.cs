using System.Collections.Generic;

public class CombatExecutor
{
    private readonly BattleRuntime _runtime;
    private readonly DiceRuleTable _ruleTable;
    private readonly CombatEventBuffer _eventBuffer;
    private readonly IRng _rng;

    public CombatExecutor(IRng rng, BattleRuntime runtime)
    {
        _ruleTable = new DiceRuleTable();
        _eventBuffer = new();
        _rng = rng;
        _runtime = runtime;
    }

    public void Execute(BoutGraph graph)
    {
        var queue = BuildQueue(graph);
        RunQueue(queue, graph);
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
            if (!IsValidAction(action))
            {
                visited.Add(slot);
                continue;
            }

            if (!graph.edges.TryGetValue(slot, out var targetSlot)) continue;
            if (!IsTargetAlive(targetSlot))
            {
                visited.Add(slot);
                continue;
            }

            if (graph.ActionBySlot.TryGetValue(targetSlot, out var opponent)
                && !IsTargetStaggered(targetSlot)
                && !visited.Contains(targetSlot))
            {
                visited.Add(slot);
                visited.Add(targetSlot);

                _eventBuffer.Push(new CombatEvent { Type = CombatEventType.UseAction, Action = action });
                _eventBuffer.Push(new CombatEvent { Type = CombatEventType.UseAction, Action = opponent });

                ResolveCombat(action, opponent);
            }
            else
            {
                visited.Add(slot);

                _eventBuffer.Push(new CombatEvent { Type = CombatEventType.UseAction, Action = action });

                ResolveCombat(action, null);
            }
        }
    }

    void ResolveCombat(ActionInstance a, ActionInstance b)
    {
        int idA = a.SourceSlot.CharacterId;
        int idB = b?.SourceSlot.CharacterId ?? -1;

        while (true)
        {
            var entryA = _runtime.PeekDice(idA);
            var entryB = b != null ? _runtime.PeekDice(idB) : null;

            if (entryA == null) break;

            if (entryB != null)
                ResolveDiceClash(idA, idB);
            else
                ResolveUnopposedDice(idA);
        }

        if (b != null)
        {
            while (_runtime.PeekDice(idB) != null)
                ResolveUnopposedDice(idB);
        }
    }

    void ResolveDiceClash(int idA, int idB)
    {
        var entryA = _runtime.PeekDice(idA).Value;
        var entryB = _runtime.PeekDice(idB).Value;

        entryA.Dice.Roll(_rng);
        entryB.Dice.Roll(_rng);

        var rule = _ruleTable.GetRule(entryA.Dice.Type, entryB.Dice.Type);
        var result = rule.Resolve(entryA.Dice, entryB.Dice);

        _runtime.AddLog(new DiceClashLog(entryA.Handle, entryB.Handle, result));

        _runtime.AdvanceDice(idA, result.AdvanceTypeA);
        _runtime.AdvanceDice(idB, result.AdvanceTypeB);
    }

    void ResolveUnopposedDice(int characterId)
    {
        var entry = _runtime.PeekDice(characterId).Value;

        if (entry.Dice.Type == DiceType.Attack)
        {
            _eventBuffer.Push(new DamageEvent(entry.Handle));
            _runtime.AdvanceDice(characterId, AdvanceType.Destroy);
        }
        else
        {
            _eventBuffer.Push(new DiceConsumedEvent(entry.Handle));
            _runtime.AdvanceDice(characterId, AdvanceType.Consume);
        }
    }

    bool IsValidAction(ActionInstance action)
    {
        var actor = _runtime.GetCharacterRuntime(action.SourceSlot.CharacterId);
        return !actor.IsDead && !actor.IsStaggered;
    }

    bool IsTargetAlive(SpeedSlot slot)
        => !_runtime.GetCharacterRuntime(slot.CharacterId).IsDead;

    bool IsTargetStaggered(SpeedSlot slot)
        => _runtime.GetCharacterRuntime(slot.CharacterId).IsStaggered;
}