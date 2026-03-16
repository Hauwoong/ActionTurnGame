
public interface IDiceRule
{
    DiceClashResult Resolve(DiceRuntime a, DiceRuntime b, IRng rng);
}