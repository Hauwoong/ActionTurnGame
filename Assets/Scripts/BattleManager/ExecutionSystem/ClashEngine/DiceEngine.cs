
public class DiceEngine
{
    public static DiceClashOutcome ResolveClash(DiceRuntime a, DiceRuntime b)
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

    public static DiceUnopposedOutcome ResolveUnopposed(DiceRuntime a)
    {
        int rollA = a.Roll();

        DiceUnopposedOutcome outcome = new DiceUnopposedOutcome
        {
            RollA = rollA,
            Damage = rollA, // 데미지 공식 계산 확장성을 위한 
        };

        outcome.DestoryA = true; // 만약 재사용 하거나 안 부숴지는 주사위시 확장성을 위한

        return outcome; 
    }
}

