
public enum DiceType
{
    Attack,
    Block,
    Envade
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
}



