
public class DiceRuleTable
{
    private readonly DiceRule[,] _table;

    public DiceRuleTable()
    {
        _table = new DiceRule[3, 3];

        Initialize();
    }

    public DiceRule GetRule(DiceType a, DiceType b)
    {
        return _table[(int)a, (int)b];
    }

    void Initialize()
    {
        var destroyBoth = (AdvanceType.Destroy, AdvanceType.Destroy);

        // Attack vs Attack
        _table[(int)DiceType.Attack, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Attack vs Block
        _table[(int)DiceType.Attack, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Block vs Attack (´ëÄª)
        _table[(int)DiceType.Block, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Attack vs Evade ¡ç À¯ÀÏÇÑ Reuse ÄÉÀÌ½º
        _table[(int)DiceType.Attack, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Evade vs Attack (´ëÄª)
        _table[(int)DiceType.Evade, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Reuse, AdvanceType.Destroy),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Block vs Block
        _table[(int)DiceType.Block, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Block vs Evade
        _table[(int)DiceType.Block, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Evade vs Block (´ëÄª)
        _table[(int)DiceType.Evade, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };

        // Evade vs Evade
        _table[(int)DiceType.Evade, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth
        };
    }
}