using System.Collections.Generic;

public class CombatExecutor
{
    private readonly BattleRuntime _runtime;
    private readonly DiceRuleTable _ruleTable;
    private readonly IRng _rng;

    public CombatExecutor(IRng rng, BattleRuntime runtime)
    {
        _ruleTable = new DiceRuleTable();
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
                ResolveCombat(action, opponent);
            }
            else
            {
                visited.Add(slot);
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
            var diceA = _runtime.PeekDice(idA);
            var diceB = b != null ? _runtime.PeekDice(idB) : null;

            if (diceA == null) break;

            if (diceB != null)
                ResolveDiceClash(idA, idB);
            else
                ResolveUnopposedDice(idA, idB);
        }

        if (b != null)
        {
            while (_runtime.PeekDice(idB) != null)
                ResolveUnopposedDice(idB, idA);
        }
    }

    void ResolveDiceClash(int idA, int idB)
    {
        var entryA = _runtime.PeekDice(idA).Value;
        var entryB = _runtime.PeekDice(idB).Value;

        var charA = _runtime.GetCharacterRuntime(idA);
        var charB = _runtime.GetCharacterRuntime(idB);

        entryA.Dice.Roll(_rng);
        entryB.Dice.Roll(_rng);

        var clashCtx = new ClashContext(entryA.Dice, entryB.Dice, charA, charB);

        charA.TriggerBeforeClash(clashCtx, isOwnerA: true);
        charB.TriggerBeforeClash(clashCtx, isOwnerA: false);

        var rule = _ruleTable.GetRule(entryA.Dice.Type, entryB.Dice.Type);
        var (result, advanceA, advanceB, ctx) = rule.Resolve(clashCtx);

        _runtime.AddLog(new DiceClashLog(
            entryA.Handle, entryB.Handle,
            entryA.Dice.CurrentRoll, entryB.Dice.CurrentRoll,
            result, advanceA, advanceB
        ));

        charA.TriggerDiceClash();
        charB.TriggerDiceClash();

        if (ctx != null)
            _runtime.EnqueueEvent(new ClashContextEvent(ctx));

        var eventA = ToAdvanceEvent(idA, advanceA);
        var eventB = ToAdvanceEvent(idB, advanceB);

        if (eventA != null) _runtime.EnqueueEvent(eventA);
        if (eventB != null) _runtime.EnqueueEvent(eventB);
    }

    void ResolveUnopposedDice(int characterId, int targetId)
    {
        var entry = _runtime.PeekDice(characterId).Value;

        if (entry.Dice.Type == DiceType.Attack)
        {
            _runtime.EnqueueEvent(new DamageEvent(characterId, targetId, entry.Dice.CurrentRoll));
            _runtime.EnqueueEvent(new DiceDestroyedEvent(characterId));
        }
        else
        {
            _runtime.EnqueueEvent(new DiceConsumedEvent(characterId));
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

    ICombatEvent ToAdvanceEvent(int characterId, AdvanceType type)
    {
        return type switch
        {
            AdvanceType.Destroy => new DiceDestroyedEvent(characterId),
            AdvanceType.Consume => new DiceConsumedEvent(characterId),
            AdvanceType.Reuse => null, // Reuse는 아무것도 안 함
            _ => null
        };
    }
}