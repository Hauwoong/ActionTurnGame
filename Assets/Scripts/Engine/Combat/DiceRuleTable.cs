public class DiceRuleTable
{
    private readonly DiceRule[,] _table;

    public DiceRuleTable()
    {
        _table = new DiceRule[4, 4];
        Initialize();
    }

    public DiceRule GetRule(DiceType a, DiceType b)
        => _table[(int)a, (int)b];

    void Initialize()
    {
        var destroyBoth = (AdvanceType.Destroy, AdvanceType.Destroy);
        var counterWin = (AdvanceType.Reuse, AdvanceType.Destroy);
        var counterLose = (AdvanceType.Destroy, AdvanceType.Destroy);


        // Attack vs Attack
        _table[(int)DiceType.Attack, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new DamageContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new DamageContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Attack vs Block
        _table[(int)DiceType.Attack, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new DamageContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB  // °ø°Ý°ª - ¼öºñ°ª
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                false
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Attack (´ëÄª)
        _table[(int)DiceType.Block, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                false
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new DamageContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Attack vs Evade
        _table[(int)DiceType.Attack, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new DamageContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                true  // È¸ÇÇ »ç¿ëÀÚ ÈåÆ®·¯Áü È¸º¹
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Attack (´ëÄª)
        _table[(int)DiceType.Evade, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Reuse, AdvanceType.Destroy),
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerA,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                true  // È¸ÇÇ »ç¿ëÀÚ ÈåÆ®·¯Áü È¸º¹
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new DamageContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Block
        _table[(int)DiceType.Block, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                false  // Áø ÂÊ ÈåÆ®·¯Áü ÇÇÇØ
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                false
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Evade
        _table[(int)DiceType.Block, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                false  // È¸ÇÇ ÂÊ ÈåÆ®·¯Áü ÇÇÇØ
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                true  // È¸ÇÇ »ç¿ëÀÚ ÈåÆ®·¯Áü È¸º¹
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Block (´ëÄª)
        _table[(int)DiceType.Evade, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerA,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                true  // È¸ÇÇ »ç¿ëÀÚ ÈåÆ®·¯Áü È¸º¹
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                false  // È¸ÇÇ ÂÊ ÈåÆ®·¯Áü ÇÇÇØ
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Evade
        _table[(int)DiceType.Evade, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerA,
                ctx.ModifiedRollA - ctx.ModifiedRollB,
                true  // ÀÌ±ä ÂÊ ÈåÆ®·¯Áü È¸º¹
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB - ctx.ModifiedRollA,
                true
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Counter vs Attack
        _table[(int)DiceType.Counter, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = counterWin,
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB),

            Lose = ClashResult.BWin,
            LoseAdvance = counterLose,
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Attack vs Counter (´ëÄª)
        _table[(int)DiceType.Attack, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Destroy, AdvanceType.Destroy),
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Counter vs Block
        _table[(int)DiceType.Counter, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = counterWin,
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB),

            Lose = ClashResult.BWin,
            LoseAdvance = counterLose,
            LoseContext = (ctx) => new StaggerContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA, false),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Counter (´ëÄª)
        _table[(int)DiceType.Block, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB, false),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Counter vs Evade
        _table[(int)DiceType.Counter, (int)DiceType.Evade] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = counterWin,
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new StaggerContext(ctx.OwnerB, ctx.OwnerB, ctx.ModifiedRollB - ctx.ModifiedRollA, true),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Counter (´ëÄª)
        _table[(int)DiceType.Evade, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Reuse, AdvanceType.Destroy),
            WinContext = (ctx) => new StaggerContext(ctx.OwnerA, ctx.OwnerA, ctx.ModifiedRollA - ctx.ModifiedRollB, true),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Counter vs Counter
        _table[(int)DiceType.Counter, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = counterWin,
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA - ctx.ModifiedRollB),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB - ctx.ModifiedRollA),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };
    }
}