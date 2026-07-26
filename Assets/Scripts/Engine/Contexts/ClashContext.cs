public class ClashContext : IClashContext
{
    public DiceRuntime DiceA { get; }
    public DiceRuntime DiceB { get; }
    public int ModifiedRollA { get; set; }
    public int ModifiedRollB { get; set; }
    public CharacterRuntime OwnerA { get; }
    public CharacterRuntime OwnerB { get; }
    public bool IsCancelled { get; set; }

    CharacterRuntime IClashContext.Defender => OwnerB;

    public ClashContext(DiceRuntime diceA, DiceRuntime diceB,
                        CharacterRuntime ownerA, CharacterRuntime ownerB)
    {
        DiceA = diceA;
        DiceB = diceB;
        OwnerA = ownerA;
        OwnerB = ownerB;
        ModifiedRollA = diceA.CurrentRoll;
        ModifiedRollB = diceB.CurrentRoll;
    }
}