
public enum DiceType
{
    Attack,
    Block,
    Evade
}

public enum DiceEffect
{
   
}

[System.Serializable]
public class DiceData
{
    public DiceType Type;
    public int Min;
    public int Max;
    public DiceEffect[] Effects;
}



