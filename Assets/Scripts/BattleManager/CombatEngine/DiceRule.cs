
public class DiceRule
{
    public ClashResult Win;
    public ClashResult Lose;
    public ClashResult Draw;

    public (AdvanceType A, AdvanceType B) WinAdvance;
    public (AdvanceType A, AdvanceType B) LoseAdvance;
    public (AdvanceType A, AdvanceType B) DrawAdvance;

    public (ClashResult Result, AdvanceType AdvanceA, AdvanceType AdvanceB) Resolve(DiceRuntime diceA, DiceRuntime diceB)
    {
        if (diceA.CurrentRoll > diceB.CurrentRoll)
            return (Win, WinAdvance.A, WinAdvance.B);
        if (diceA.CurrentRoll < diceB.CurrentRoll)
            return (Lose, LoseAdvance.A, LoseAdvance.B);
        return (Draw, DrawAdvance.A, DrawAdvance.B);
    }
}