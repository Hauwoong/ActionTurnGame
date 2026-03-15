using System.Collections.Generic;

public class CombatExecutor
{
    private readonly BattleRuntime runtime;
    private DiceRuleTable ruleTable;
    private readonly IRng rng;

    public CombatExecutor(DiceRuleTable ruleTable, IRng rng, BattleRuntime runtime)
    {
        this.ruleTable = ruleTable;
        this.rng = rng;
        this.runtime = runtime;
    }
  
    public void Execute(BoutGraph graph)
    {
        var queue = BuildQueue(graph);

        RunQueue(queue,graph);
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

            if (graph.edges.TryGetValue(slot, out var targetSlot))
            {
                if (!IsTargetAlive(targetSlot))
                {
                    visited.Add(slot);
                    continue;
                }

                if (graph.ActionBySlot.TryGetValue(targetSlot, out var opponent) && !IsTargetStaggered(targetSlot) && !visited.Contains(targetSlot))
                {
                    visited.Add(slot);
                    visited.Add(targetSlot);

                    runtime.UseAction(action); // 이벤트 형식으로 바꿔서 로그 -> 런타임 적용으로 개선 해야 함
                    runtime.UseAction(opponent);

                    ResolveCombat(action, opponent);
                }

                else
                {
                    visited.Add(slot);

                    runtime.UseAction(action);

                    ResolveCombat(action, null);
                }
            }
        }
    }

    void ResolveCombat(ActionInstance a, ActionInstance b)
    {
        var diceA = runtime.GetRemainingDice(a.SourceSlot.CharacterId).GetEnumerator();
        var diceB = b != null
            ? runtime.GetRemainingDice(b.SourceSlot.CharacterId).GetEnumerator()
            : null;

        bool hasA = diceA.MoveNext();
        bool hasB = diceB != null && diceB.MoveNext();

        while (hasA && hasB)
        {
            ResolveDiceClash(diceA.Current, diceB.Current);

            hasA = diceA.MoveNext();
            hasB = diceB.MoveNext();
        }

        while (hasA)
        {
            ResolveUnopposedDice(diceA.Current);

            hasA = diceA.MoveNext();
        }

        while (hasB)
        {
            ResolveUnopposedDice(diceB.Current);

            hasB = diceB.MoveNext();
        }
    }

    void ResolveDiceClash(DiceEntry a, DiceEntry b)
    {
        DiceRuntime diceA = a.Dice;
        DiceRuntime diceB = b.Dice;

        diceA.Roll(rng);
        diceB.Roll(rng);

        var rule = ruleTable.Get(diceA.GetDiceType(), diceB.GetDiceType());

        rule.Resolve(diceA, diceB, context); // 이때 context는 damagecontext일까? 일단 넘기자
    }

    bool IsValidAction(ActionInstance action)
    {
        var actor = runtime.GetCharacterRuntime(action.SourceSlot.CharacterId);

        if (actor.IsDead) return false;
        if (actor.IsStaggered) return false;

        return true;
    }

    bool IsTargetAlive(SpeedSlot slot)
    {
        var character = runtime.GetCharacterRuntime(slot.CharacterId);

        return !character.IsDead;
    }

    bool IsTargetStaggered(SpeedSlot slot)
    {
        var character = runtime.GetCharacterRuntime(slot.CharacterId);

        return character.IsStaggered;
    }
}
