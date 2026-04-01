using System.Collections.Generic;

public sealed class CharacterState
{
    public CharacterData Source { get; }
    public int CharacterId { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int MaxEnergy { get; }
    public int SpeedSlotCount { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }
    public int MaxEmotionStack { get; }
    public int MaxEmotionLevel { get; }
    // 감정 스택 쌓는 방법 -> 패시브 감정 스택 쌓는 거 증가 있으면 증가 시켜야 함 즉 snapshot에서 state 만들때 해당 자원에 해당하는 패시브가 있으면 적용해서 만들어야 함. 그럼에도 굳이 패시브를 남겨놓는 이유는 패시브가 자원에 해당하는 패시브만 목록에 있지 않기 때문 그렇다면 statuseffect뿐만 아니라 passiveeffect trigger또한 만들어야 함
    public int EmotionGainOnDamageDealt { get; }
    public int EmotionGainOnDamageReceived { get; }
    public int EmotionGainOnBlockSuccess { get; }
    public int EmotionGainOnEvadeSuccess { get; }
    public IReadOnlyList<PassiveData> Passives { get; }


    public CharacterState(CharacterData source, int id)
    {
        Source = source;
        CharacterId = id;
        MaxHp = source.MaxHp;
        MaxStagger = source.MaxStagger;
        MaxEnergy = source.MaxEnergy;
        SpeedSlotCount = source.BaseSpeedSlotCount;
        MinSpeed = source.MinSpeed;
        MaxSpeed = source.MaxSpeed;
        MaxEmotionStack = source.MaxEmotionStack;
        MaxEmotionLevel = source.MaxEmotionLevel;
        EmotionGainOnDamageDealt = source.EmotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = source.EmotionGainOnDamageReceived;
        EmotionGainOnBlockSuccess = source.EmotionGainOnBlockSuccess;
        EmotionGainOnEvadeSuccess = source.EmotionGainOnEvadeSuccess;

        // 패시브 적용 (자원 수정 패시브만)
        var passives = new List<PassiveData>(source.Passives);
        foreach (var passive in passives)
        {
            if (passive is StatModifierPassive stat)
            {
                // 스탯 수정 패시브 적용
                // 예: MaxHp += stat.Value 등
            }
        }
        Passives = passives;
    }


}