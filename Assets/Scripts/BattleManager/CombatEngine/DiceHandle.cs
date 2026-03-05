public readonly struct DiceHandle
{
    public readonly int OwnerId;
    public readonly int DiceId;

    public DiceHandle(int ownerId, int diceId)
    {
        OwnerId = ownerId;
        DiceId = diceId;
    }
}
