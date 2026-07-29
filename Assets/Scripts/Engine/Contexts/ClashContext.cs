public class ClashContext
{
    public int ModifiedRollA { get; set; }
    public int ModifiedRollB { get; set; }
    public CharacterRuntime OwnerA { get; }
    public CharacterRuntime OwnerB { get; }
    public bool IsCancelled { get; set; }

    public ClashContext(CharacterRuntime ownerA, CharacterRuntime ownerB, int modifyedRollA, int modifyedRollB)
    {
       
        OwnerA = ownerA;
        OwnerB = ownerB;
        ModifiedRollA = modifyedRollA;
        ModifiedRollB = modifyedRollB;
    }
}