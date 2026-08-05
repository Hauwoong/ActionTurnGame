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
                CharacterId = slot.CharacterId,
                SlotIndex = slot.SlotIndex
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

            bool hasEdge = graph.edges.TryGetValue(slot, out var targetSlot);

            if (!hasEdge)
            {
                targetSlot = action.TargetSlot;
            }

            if (!IsAlive(targetSlot.CharacterId))
            {
                visited.Add(slot);
                continue;
            }

            if (graph.ActionBySlot.TryGetValue(targetSlot, out var opponent)
                && !IsTargetStaggered(targetSlot)
                && !visited.Contains(targetSlot)
                && hasEdge)
            {
                visited.Add(slot);
                visited.Add(targetSlot);
                _runtime.EnqueueEvent(new BoutStartEvent(action, opponent, slot.CharacterId, targetSlot.CharacterId));
            }
            else
            {
                visited.Add(slot);
                _runtime.EnqueueEvent(new BoutStartEvent(action, null, slot.CharacterId, targetSlot.CharacterId));
            }
        }
    }

    public void ResolveCombat(int attackerId, int targetId)
    {
        while (true)
        {
            var diceA = _runtime.PeekDice(attackerId);

            if (diceA == null) break;

            if (!IsAlive(targetId)) 
            {
                _runtime.EnqueueEvent(new DiceDiscardRemainingEvent(attackerId));
                break;
            }

            var diceB = _runtime.PeekDice(targetId);

            if (diceB != null)
                ResolveDiceClash(attackerId, targetId);
            else
                ResolveUnopposedDice(attackerId, targetId);
        }
        
        while (_runtime.PeekDice(targetId) != null)
        {
            if (!IsAlive(attackerId))
            {
                _runtime.EnqueueEvent(new DiceDiscardRemainingEvent(targetId));
                break;
            }

            ResolveUnopposedDice(targetId, attackerId);
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

        var modifyedRollA = charA.TriggerModifyRoll(entryA.Dice);
        var modifyedRollB = charB.TriggerModifyRoll(entryB.Dice);

        charA.TriggerDiceRoll();
        charB.TriggerDiceRoll();

        var clashCtx = new ClashContext(charA, charB, modifyedRollA, modifyedRollB);

        var rule = _ruleTable.GetRule(entryA.Dice.Type, entryB.Dice.Type);
        var (result, advanceA, advanceB, ctx) = rule.Resolve(clashCtx);

        _runtime.AddLog(new DiceClashLog(
            entryA.Handle, entryB.Handle,
            entryA.Dice.CurrentRoll, entryB.Dice.CurrentRoll,
            result, advanceA, advanceB
        ));

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

        if (entry.Dice.Type != DiceType.Attack)
        {
            _runtime.EnqueueEvent(new DiceConsumedEvent(characterId));
            return;
        }

        var attacker = _runtime.GetCharacterRuntime(characterId);
        var target = _runtime.GetCharacterRuntime(targetId);

        entry.Dice.Roll(_rng); // 주사위 굴리기 추가

        var modifyedRoll = attacker.TriggerModifyRoll(entry.Dice);

        attacker.TriggerDiceRoll();

        if (attacker.IsDead) return;

        var ctx = new DamageContext(attacker, target, modifyedRoll);
        _runtime.EnqueueEvent(new DamageEvent(ctx));
        _runtime.EnqueueEvent(new DiceDestroyedEvent(characterId));
       
    }

    bool IsValidAction(ActionInstance action)
    {
        var actor = _runtime.GetCharacterRuntime(action.SourceSlot.CharacterId);
        return !actor.IsDead && !actor.IsStaggered;
    }

    bool IsAlive(int characterId) => !_runtime.GetCharacterRuntime(characterId).IsDead;

    bool IsTargetStaggered(SpeedSlot slot)
        => _runtime.GetCharacterRuntime(slot.CharacterId).IsStaggered;

    ICombatEvent ToAdvanceEvent(int characterId, AdvanceType type)
    {
        return type switch
        {
            AdvanceType.Destroy => new DiceDestroyedEvent(characterId),
            AdvanceType.Consume => new DiceConsumedEvent(characterId),
            AdvanceType.Reuse => new DiceReusedEvent(characterId),
            _ => null
        };
    }
}