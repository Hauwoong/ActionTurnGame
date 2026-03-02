using System.Collections.Generic;

public readonly struct ClashInput
{
    public readonly IReadOnlyList<DiceEntry> DiceA;
    public readonly IReadOnlyList<DiceEntry> DiceB;

    public ClashInput(IReadOnlyList<DiceEntry> diceA, IReadOnlyList<DiceEntry> diceB)
    {
        DiceA = diceA;
        DiceB = diceB;
    }
}
