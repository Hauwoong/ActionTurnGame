using UnityEngine;

public sealed class CharacterState
{
    public Character Source;

    public int MaxHp { get; }

    public CharacterState(Character source, int maxHp, int speed, int attackPower)
    {
        Source = source;
        MaxHp = maxHp;
    }


}