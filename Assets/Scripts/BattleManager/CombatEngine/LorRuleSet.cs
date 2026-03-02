using System.Collections.Generic;

public class LorRuleSet : IRuleSet
{
    public bool CanContinue(ClashInput input, int cursorA, int cursorB)
    {
        return cursorA < input.DiceA.Count &&
               cursorB < input.DiceB.Count;
    }

    public ResolveStepResult ResolveStep(ClashInput input, int cursorA, int cursorB, IRng rng)
    {
        var entryA = input.DiceA[cursorA];
        var entryB = input.DiceB[cursorB];

        var outcome = DiceEngine.ResolveClash(entryA, entryB, rng);

        var events = BuildEvents(input, entryA, entryB, outcome);

        return new ResolveStepResult(events,
            cursorA + (outcome.DestoryA ? 1 : 0),
            cursorB + (outcome.DestoryB ? 1 : 0));
    }

    private List<ICombatEvent> BuildEvents(
        ClashInput input,
        DiceEntry entryA,
        DiceEntry entryB,
        DiceClashOutcome outcome)
    {
        var events = new List<ICombatEvent>();


    }
}
