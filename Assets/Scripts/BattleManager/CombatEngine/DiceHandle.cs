public readonly struct DiceHandle
{
    public readonly Character Owner;
    public readonly int DiceId;

    public DiceHandle(Character owner, int diceId)
    {
        Owner = owner;
        DiceId = diceId;
    }
}
