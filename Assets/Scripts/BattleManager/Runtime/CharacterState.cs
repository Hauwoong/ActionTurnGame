
public sealed class CharacterState
{
    public Character Source { get; }

    public int CharacterId { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int SpeedSlotCount { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }

    public CharacterState(Character source, int id)
    {
        Source = source;
        CharacterId = id;
        MaxHp = source.MaxHP;
        MaxStagger = source.MaxStagger;
        SpeedSlotCount = source.diceCount;
        MinSpeed = source.MinSpeed;
        MaxSpeed = source.MaxSpeed;
    }


}