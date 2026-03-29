public sealed class TurnStartContext
{
    public CharacterRuntime Character { get; }

    public TurnStartContext(CharacterRuntime character)
    {
        Character = character;
    }
}
