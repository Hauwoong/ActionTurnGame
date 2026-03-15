
using System.Collections.Generic;

public class DiceRuleTable
{
    private readonly Dictionary<(DiceType, DiceType), IDiceRule> rules;

    public IDiceRule Get(DiceType a, DiceType b)
    {
        return rules[(a, b)];
    }
}