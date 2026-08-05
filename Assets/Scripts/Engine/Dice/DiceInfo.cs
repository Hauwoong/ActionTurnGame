
public readonly struct DiceInfo
{
    public DiceType Type { get; }
    public int Min { get; }
    public int Max { get; }
    public DiceInfo(DiceType type, int min, int max)
    {
        Type = type;
        Min = min;
        Max = max;
    }
}
