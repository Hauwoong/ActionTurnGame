public class CombatExecutor
{
    private readonly BattleRuntime runtime;
    private readonly IRuleSet rules;
    private readonly IRng rng;

    public CombatExecutor(IRuleSet rules, IRng rng, BattleRuntime runtime)
    {
        this.rules = rules;
        this.rng = rng;
        this.runtime = runtime;
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
                runtime.EnqueueEvent(ev);
            }

            cursorA = step.NextCursorA;
            cursorB = step.NextCursorB;
        }
    }
}
