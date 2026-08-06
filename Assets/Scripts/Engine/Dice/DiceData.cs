
public enum DiceType
{
    Attack,
    Block,
    Evade,
    Counter
}

public enum DiceEffect
{
   
}

public static class DiceTypeExtensions
{
    /// <summary>
    /// 공격형 = 데미지를 내는 주사위 (Attack, Counter). Attack만 필요한 자리에는 쓰지 말것
    /// </summary>
    public static bool IsOffensive(this DiceType type)
        => type == DiceType.Attack || type == DiceType.Counter;
}

[System.Serializable]
public class DiceData
{
    public DiceType Type;
    public int Min;
    public int Max;
    public DiceEffect[] Effects;
}



