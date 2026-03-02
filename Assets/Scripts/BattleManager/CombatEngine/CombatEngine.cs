
public class CombatEngine
{
    public static ClashResult Resolve(
        ClashInput input,
        IRng rng)
    {
        var result = new ClashResult();

        int cursorA = 0;
        int cursorB = 0;

        while (cursorA < input.DiceA.Count &&
               cursorB < input.DiceB.Count)
        {
            var diceA = input.DiceA[cursorA];
            var diceB = input.DiceB[cursorB];

            var outcome = DiceEngine.ResolveClash(diceA, diceB, rng);

            result.Steps.Add(new ClashStepResult(
                outcome.RollA,
                outcome.RollB,
                outcome.DestoryA,
                outcome.DestoryB));

            if (outcome.DestoryA) cursorA++;
            if (outcome.DestoryB) cursorB++;
        }

        result.AFinished = cursorA >= input.DiceA.Count;
        result.BFinished = cursorB >= input.DiceB.Count;

        return result;
    }

}
