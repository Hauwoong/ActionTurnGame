using UnityEngine;

public sealed class CharacterState
{
    public Character Source { get; }

    public int CharacterId { get; }
    public int MaxHp { get; }

    public CharacterState(Character source, int id)
    {
        Source = source;
        CharacterId = id;
        MaxHp = source.MaxHP;
    }


}