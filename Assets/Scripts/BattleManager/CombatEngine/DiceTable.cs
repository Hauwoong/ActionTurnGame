using System.Collections.Generic;

public class DiceRuleTable
{
    private readonly DiceRule[,] table;

    public DiceRuleTable()
    {
        table = new DiceRule[3, 3];

        Initialize();
    }

    public DiceRule GetRule(DiceType a, DiceType b)
    {
        return table[(int)a, (int)b];
    }

    void Initialize()
    {
        table[(int)DiceType.Attack, (int)DiceType.Attack] =
            new DiceRule
            {
                Win = ClashResult.AWin,
                Lose = ClashResult.BWin,
                Draw = ClashResult.Draw
            };
    }
}