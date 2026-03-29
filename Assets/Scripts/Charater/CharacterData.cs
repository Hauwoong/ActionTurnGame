using System.Collections.Generic;

public class CharacterData
{
    public string Name { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int MaxEnergy { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }
    public int BaseSpeedSlotCount { get; } // 기본 스피드 슬롯 수 (디폴트 1)

    // 감정 단계 획득 조건
    public int EmotionGainOnDamageDealt { get; }    // 피해를 줄 때
    public int EmotionGainOnDamageReceived { get; } // 피해를 받을 때
    public int EmotionGainOnBlockSuccess { get; }   // 방어 성공 시
    public int EmotionGainOnEvadeSuccess { get; }   // 회피 성공 시
    public int MaxEmotionLevel { get; }             // 감정 단계 최대치
    public IReadOnlyList<PassiveData> Passives { get; }

    public CharacterData(
        string name,
        int maxHp,
        int maxStagger,
        int maxEnergy,
        int minSpeed,
        int maxSpeed,
        int baseSpeedSlotCount = 1,
        List<PassiveData> passives = null)
    {
        Name = name;
        MaxHp = maxHp;
        MaxStagger = maxStagger;
        MaxEnergy = maxEnergy;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
        BaseSpeedSlotCount = baseSpeedSlotCount;
        Passives = passives ?? new List<PassiveData>();
    }
}
