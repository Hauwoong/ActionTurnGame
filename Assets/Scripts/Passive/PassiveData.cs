public abstract class PassiveData
{
    public string Name { get; }
    public PassiveType Type { get; }

    protected PassiveData(string name, PassiveType type)
    {
        Name = name;
        Type = type;
    }
}

public enum PassiveType
{
    SpeedSlot,      // 스피드 슬롯 관련
    AttackBoost,
    // 나중에 추가
}