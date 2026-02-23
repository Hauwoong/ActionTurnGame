
public class DiceEngine
{
    public static DiceClashOutcome Resolve(DiceRuntime a, DiceRuntime b)
    {
        int rollA = a.Roll();
        int rollB = b.Roll();

        DiceClashOutcome outcome = new DiceClashOutcome
        {
            RollA = rollA,
            RollB = rollB
        };

        if (a.Type == DiceType.Envade && b.Type == DiceType.Envade)
        {
            outcome.DestoryA = true;
            outcome.DestoryB = true;
        }

        else if (rollA > rollB && a.Type == DiceType.Envade)
        {
            outcome.DestoryA = false;
            outcome.DestoryB = true;
        }

        else if (rollA < rollB && b.Type == DiceType.Envade)
        {
            outcome.DestoryA = true;
            outcome.DestoryB = false;
                                 
        }

        else
        {
            outcome.DestoryA = true;
            outcome.DestoryB = true;
        }

        return outcome;
    }
}

