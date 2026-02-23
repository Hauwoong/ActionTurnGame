
public class ClashEngine
{
    public static ClashResult Resolve(ActionRuntime a, ActionRuntime b)
    {
        ClashResult result = new ClashResult
        {
            Attacker = a.Source.SourceSlot,
            Defender = b.Source.SourceSlot,
        };

        while (!a.IsFinished && !b.IsFinished)
        {
            DiceRuntime diceA = a.CurrentDice;
            DiceRuntime diceB = b.CurrentDice;

            DiceClashOutcome outcome = DiceEngine.Resolve(diceA, diceB);

            ClashStepResult step = new ClashStepResult
            {
                AttackerDiceIndex = a.CurrentIndex,
                DefenderDiceIndex = b.CurrentIndex,
                AttackerDiceRoll = outcome.RollA,
                DefenderDiceRoll = outcome.RollB,
                Outcome = Outcome(outcome.RollA, outcome.RollB)
            };

            result.Steps.Add(step);

            if (outcome.DestoryA)
            {
                diceA.IsDestoryed = true;
                a.Advance();
            }

            if (outcome.DestoryB)
            {
                diceB.IsDestoryed = true;
                b.Advance();
            }
        }

        result.AttackerWon = !a.IsFinished && b.IsFinished;
        result.DefenderWon = a.IsFinished && !b.IsFinished;

        result.AttackerRemainingDice = a.RemainingDiceCount;
        result.DefenderRemainingDice = b.RemainingDiceCount;

        return result;
    }

    public static 

    static ClashStepOutcome Outcome(int a, int b)
    {
        if (a > b) return ClashStepOutcome.AttackerWin;
        if (b > a) return ClashStepOutcome.DefenderWin;
        return ClashStepOutcome.Draw;
    }

}


