using System.Collections.Generic;

public class CharacterRuntime
{
    public Character Owner;

    private List<DiceEntry> DicePool = new();

    private Dictionary<int, DiceRuntime> DiceById = new();

    public int DiceCursor { get; private set; }

    private int NextDiceId = 0;

    public bool IsFinished => DiceCursor >= DicePool.Count;

    public CharacterRuntime(Character owner)
    {
        Owner = owner;
        DiceCursor = 0;
    }

    public IReadOnlyList<DiceEntry> GetRemainingDice() => DicePool;

    public DiceEntry GetCurrentDice()
    {
        if (IsFinished) return null;
        return DicePool[DiceCursor];
    }

    public DiceEntry GetDiceAt(int index) => DicePool[index];

    public void AddCardDice(List<DiceEntry> cardDice)
    {
        DicePool.InsertRange(DiceCursor, cardDice);
    }
    
    public void MarkDestoryed(int DiceId)
    {
        DicePool[DiceCursor].Dice.IsDestoryed = true;
    }

    public void Advance()
    {
        while (!IsFinished &&
                DicePool[DiceCursor].Dice.IsDestoryed)
        {
            DiceCursor++;
        }
    }
    
    public void ResetCursor()
    {
        DiceCursor = 0;
    }
}
