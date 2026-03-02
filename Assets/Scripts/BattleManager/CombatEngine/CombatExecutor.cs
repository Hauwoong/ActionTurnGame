using System.Collections.Generic;

public class CombatExecutor
{
    private readonly IRuleSet rules;
    private readonly IRng rng;
    private readonly BattleResolutionLoop loop;

    public CombatExecutor(IRuleSet rules, IRng rng, BattleResolutionLoop loop)
    {
        this.rules = rules;
        this.rng = rng;
        this.loop = loop;
    }

    public void Execute(ClashInput input)
    {
        int cursorA = 0;
        int cursorB = 0;

        while (rules.CanContinue(input, cursorA, cursorB))
        {
            var step = rules.ResolveStep(
                input,
                cursorA,
                cursorB,
                rng);

            foreach (var ev in step.Events)
            {
                ev.Dispatch(loop);
            }

            cursorA = step.NextCursorA;
            cursorB = step.NextCursorB;
        }
    }
}
