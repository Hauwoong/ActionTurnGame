using System.Collections.Generic;

public class CharacterRuntime
{
    public Character Owner;

    private List<DiceRuntime> DicePool = new();

    public int DiceCursor;

    public bool IsFinished => DiceCursor >= DicePool.Count;

    public DiceRuntime CurrentDice => DicePool[DiceCursor];

    public void AddCardDice(List<DiceRuntime> cardDice)
    {
        DicePool.InsertRange(DiceCursor, cardDice);
    }

    public void SkipDice()
    {
        DiceCursor++;
    }
    
    public void Advance()
    {
        if (CurrentDice.IsDestoryed && !IsFinished)
        {
            DiceCursor++;
        }
    }

    public void CleanupAfterAction()
    {
        DicePool.RemoveAll(d => d.IsDestoryed);
        DiceCursor = 0;
    }

    public CharacterRuntime(Character owner)
    {
        Owner = owner;
        DiceCursor = 0;
    }
}
