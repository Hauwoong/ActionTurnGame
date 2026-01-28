using UnityEngine;

[System.Serializable]

public enum DiceType
{
    Attack,
    Block,
    Counter,
    Evade,
    Support
}

public enum DiceEffect
{
    None,
    Burn,
    Bleed,
    Stagger,
    Draw,
    Heal
}
public class DiceData
{
    public DiceType type;
    public int min;
    public int max;
    public DiceEffect effect;

    public int Roll()
    {
        return Random.Range(min, max+1);
    }
}

