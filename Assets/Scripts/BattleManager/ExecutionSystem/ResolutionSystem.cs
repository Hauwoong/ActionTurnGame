using System.Collections.Generic;

public class ResolutionSystem
{
    private BattleState state;
    private readonly CombatExecutor executor;
    private readonly BattleResolutionLoop loop;
    private readonly PriorityQueue<SpeedSlot, ActionPriority> executionQueue = new();
    private HashSet<SpeedSlot> resolved = new();

    public ResolutionSystem(BattleState state)
    {
        this.state = state;
        executor = new CombatExecutor(state.Rules, state.Rng, loop);
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

    void ResolveClash(SpeedSlot aSlot, SpeedSlot bSlot)
    {
        EnqueueCardDice(aSlot);
        EnqueueCardDice(bSlot);

        CharacterRuntime charA = state.GetRuntime(aSlot.owner);
        CharacterRuntime charB = state.GetRuntime(bSlot.owner);

        var input = new ClashInput(
            charA.GetRemainingDice(),
            charB.GetRemainingDice());

        executor.Execute(input);

        loop.Resolve();
    }

    void ResolveUnopposed(SpeedSlot slot)
    {

    }

    void EnqueueCardDice(SpeedSlot slot)
    {
        if (state.ActionBySlot.TryGetValue(slot, out var action))
        {
            var runtime = state.GetRuntime(slot.owner);

            List<DiceRuntime> cardDice = new();

            foreach (var dice in action.Card.dices)
            {
                cardDice.Add(new DiceRuntime
                {
                    Type = dice.type,
                    Min = dice.min,
                    Max = dice.max,
                    IsDestoryed = false
                });
            }

            runtime.AddCardDice(cardDice);
        }
    }   
}
