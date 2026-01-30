using UnityEngine;
public enum DiceType
{
    Attack,
    Block
}

public enum DiceEffect
{
   
}

[System.Serializable]
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

public class DiceResult
{
    public DiceType type;
    public int value;
    public Player owner;
}