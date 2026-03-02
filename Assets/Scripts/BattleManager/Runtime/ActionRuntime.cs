using System.Collections.Generic;

public class ActionRuntime
{
    public ActionInstance Source;

    public List<DiceRuntime> DicePool;

    public int CurrentIndex;
    public int RemainingDiceCount => CurrentIndex < DicePool.Count ? DicePool.Count - CurrentIndex : 0;
    public bool IsFinished => CurrentIndex >= DicePool.Count;

    public DiceRuntime CurrentDice => CurrentIndex < DicePool.Count ? DicePool[CurrentIndex] : null;

    public void Advance()
    {
        if (DicePool[CurrentIndex].IsDestoryed)
        {
            CurrentIndex++;
        }
    }
}
