public readonly struct DiceHandle
{
    public readonly CharacterHandle Owner;
    public readonly int DiceId;

    public DiceHandle(CharacterHandle owner, int diceId)
    {
        Owner = owner;
        DiceId = diceId;
    }
}
