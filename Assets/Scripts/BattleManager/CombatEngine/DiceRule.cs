
public class DiceRule
{
    public ClashResult Win;
    public ClashResult Lose;
    public ClashResult Draw;

    public AdvanceType AdvanceTypeA;
    public AdvanceType AdvanceTypeB;

    public ClashResult Resolve(DiceRuntime diceA, DiceRuntime diceB)
    {
        if (diceA.CurrentRoll > diceB.CurrentRoll)
            return Win;

        if (diceA.CurrentRoll < diceB.CurrentRoll)
            return Lose;

        return Draw;
    }
}