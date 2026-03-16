
public class AttackVSAttack : IDiceRule
{
    public DiceClashResult Resolve(DiceRuntime a, DiceRuntime b, IRng rng)
    {
        a.Roll(rng);
        b.Roll(rng);

        if (a.CurrentRoll > b.CurrentRoll)
        {
            return new DiceClashResult
            {
                RollA = a.CurrentRoll,
                RollB = b.CurrentRoll,
                DestoryA = true,
                DestoryB = true,
                AdvanceA = true,
                AdvanceB = true
            };
        }

        else if (a.CurrentRoll < b.CurrentRoll)
        {
            return new DiceClashResult
            {
                RollA = a.CurrentRoll,
                RollB = b.CurrentRoll,
                DestoryA = true,
                DestoryB = true,
                AdvanceA = true,
                AdvanceB = true
            };
        }

        return new DiceClashResult
        {
            RollA = a.CurrentRoll,
            RollB = b.CurrentRoll,
            DestoryA = true,
            DestoryB = true,
            AdvanceA = true,
            AdvanceB = true
        };
    }
}
