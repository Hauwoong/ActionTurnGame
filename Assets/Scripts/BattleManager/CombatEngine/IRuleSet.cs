public interface IRuleSet
{
    bool CanContinue(
        ClashInput input,
        int cursorA,
        int cursorB);

    ResolveStepResult ResolveStep(
        ClashInput input,
        int cursorA,
        int cursorB,
        IRng rng);
}