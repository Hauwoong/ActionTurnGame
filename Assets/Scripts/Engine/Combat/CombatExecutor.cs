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
                Speed = _runtime.GetSlotRuntime(slot).Speed,
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
            modifyedRollA, modifyedRollB,
            advanceA, advanceB, result
        ));

        if (ctx != null)
            _runtime.EnqueueEvent(new ClashContextEvent(ctx));

        var eventA = ToAdvanceEvent(idA, advanceA);
        var eventB = ToAdvanceEvent(idB, advanceB);

        if (eventA != null) _runtime.EnqueueEvent(eventA);
        if (eventB != null) _runtime.EnqueueEvent(eventB);
    }

    /// <summary>
    /// 합 상대가 없는 주사위 하나를 해석한다. 공격 주사위면 굴려서 데미지를 내고, 나머지는 굴리지 않고 저장분으로 남긴다.
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="targetId"></param>
    void ResolveUnopposedDice(int characterId, int targetId)
    {
        var entry = _runtime.PeekDice(characterId).Value;

        // 이 자리에 도달했다는 것 자체가 "대상 풀이 비었다" = 맞붙을 상대가 없다는 뜻이다.
        // 그래서 이 검사가 곧 저장 규칙이 된다 - DicePool.DiscardRemaining의 Attack 검사와 같은 뜻이고,
        // 마찬가지로 IsOffensive()로 바구면 안 된다(Counter가 저장 대상인데 소멸해버린다).
        //
        // 3-1.9 결정: 일방에서는 공격 주사위만 굴린다. 방어·회피는 어차피 저장됐다 나중에 다시 굴려지므로
        // 지금 굴린 값은 버려지는데, 버릴 굴림에 출혈 같은 굴림 훅만 터진다.
        // 원칙 - Roll() 이 있는 자리에 굴림 훅이 따라붙는다. 굴리지 않으면 훅도 없다. 
        if (entry.Dice.Type != DiceType.Attack)
        {
            _runtime.EnqueueEvent(new DiceConsumedEvent(characterId));
            return;
        }

        var attacker = _runtime.GetCharacterRuntime(characterId);
        var target = _runtime.GetCharacterRuntime(targetId);

        entry.Dice.Roll(_rng);

        var modifiedRoll = attacker.TriggerModifyRoll(entry.Dice);

        attacker.TriggerDiceRoll();

        // 주사위를 전진시키지 않고 빠지는 유일한 경로다. 그런데도 ResolveCombat의 while이 안 도는 이유는
        // 이 파일이 아니라 DeathEvent의 DestroyRemainingDice()에 있다 - 죽는 순간 남은 주사위가 전부 소멸하고
        // 다음 Peek이 null을 준다. "while의 모든 탈출 경로가 주사위를 전진시킨다"는 규칙의 유일한 예외.
        if (attacker.IsDead) return;

        _runtime.AddLog(new UnopposedLog(entry.Handle, targetId, entry.Dice.CurrentRoll, modifiedRoll));

        var ctx = new DamageContext(attacker, target, modifiedRoll);
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