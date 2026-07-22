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
                ctx.ModifiedRollA
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new DamageContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB
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
                ctx.ModifiedRollA - ctx.ModifiedRollB  // 공격값 - 수비값
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB,
                false
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Attack (대칭)
        _table[(int)DiceType.Block, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerB,
                ctx.ModifiedRollA,
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
                ctx.ModifiedRollA
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB,
                true  // 회피 사용자 흐트러짐 회복
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Attack (대칭)
        _table[(int)DiceType.Evade, (int)DiceType.Attack] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Reuse, AdvanceType.Destroy),
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerA,
                ctx.ModifiedRollA,
                true  // 회피 사용자 흐트러짐 회복
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new DamageContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB
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
                ctx.ModifiedRollA,
                false  // 진 쪽 흐트러짐 피해
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB,
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
                ctx.ModifiedRollA,
                false  // 회피 쪽 흐트러짐 피해
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB,
                true  // 회피 사용자 흐트러짐 회복
            ),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Block (대칭)
        _table[(int)DiceType.Evade, (int)DiceType.Block] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(
                ctx.OwnerA, ctx.OwnerA,
                ctx.ModifiedRollA,
                true  // 회피 사용자 흐트러짐 회복
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerA,
                ctx.ModifiedRollB,
                false  // 회피 쪽 흐트러짐 피해
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
                ctx.ModifiedRollA,
                true  // 이긴 쪽 흐트러짐 회복
            ),
            Lose = ClashResult.BWin,
            LoseAdvance = destroyBoth,
            LoseContext = (ctx) => new StaggerContext(
                ctx.OwnerB, ctx.OwnerB,
                ctx.ModifiedRollB,
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
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA),

            Lose = ClashResult.BWin,
            LoseAdvance = counterLose,
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Attack vs Counter (대칭)
        _table[(int)DiceType.Attack, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Destroy, AdvanceType.Destroy),
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB),
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
            LoseContext = (ctx) => new StaggerContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB, false),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Block vs Counter (대칭)
        _table[(int)DiceType.Block, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = destroyBoth,
            WinContext = (ctx) => new StaggerContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA, false),

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
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new StaggerContext(ctx.OwnerB, ctx.OwnerB, ctx.ModifiedRollB, true),

            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Evade vs Counter (대칭)
        _table[(int)DiceType.Evade, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = (AdvanceType.Reuse, AdvanceType.Destroy),
            WinContext = (ctx) => new StaggerContext(ctx.OwnerA, ctx.OwnerA, ctx.ModifiedRollA, true),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };

        // Counter vs Counter
        _table[(int)DiceType.Counter, (int)DiceType.Counter] = new DiceRule
        {
            Win = ClashResult.AWin,
            WinAdvance = counterWin,
            WinContext = (ctx) => new DamageContext(ctx.OwnerA, ctx.OwnerB, ctx.ModifiedRollA),

            Lose = ClashResult.BWin,
            LoseAdvance = (AdvanceType.Destroy, AdvanceType.Reuse),
            LoseContext = (ctx) => new DamageContext(ctx.OwnerB, ctx.OwnerA, ctx.ModifiedRollB),
            Draw = ClashResult.Draw,
            DrawAdvance = destroyBoth,
            DrawContext = null
        };
    }
}