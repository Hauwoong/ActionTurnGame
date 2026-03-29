
using System;

public class DiceRule
{
    public ClashResult Win;
    public ClashResult Lose;
    public ClashResult Draw;

    public (AdvanceType A, AdvanceType B) WinAdvance;
    public (AdvanceType A, AdvanceType B) LoseAdvance;
    public (AdvanceType A, AdvanceType B) DrawAdvance;

    public Func<ClashContext, IClashContext> WinContext;
    public Func<ClashContext, IClashContext> LoseContext;
    public Func<ClashContext, IClashContext> DrawContext;

    public (ClashResult Result, AdvanceType AdvanceA, AdvanceType AdvanceB, IClashContext Context)
        Resolve(ClashContext clashCtx)
    {
        if (clashCtx.ModifiedRollA > clashCtx.ModifiedRollB)
            return (Win, WinAdvance.A, WinAdvance.B, WinContext?.Invoke(clashCtx));
        if (clashCtx.ModifiedRollA < clashCtx.ModifiedRollB)
            return (Lose, LoseAdvance.A, LoseAdvance.B, LoseContext?.Invoke(clashCtx));
        return (Draw, DrawAdvance.A, DrawAdvance.B, DrawContext?.Invoke(clashCtx));
    }
}