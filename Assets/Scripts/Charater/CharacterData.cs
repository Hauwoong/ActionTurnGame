using System.Collections.Generic;

public class CharacterData
{
    public string Name { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int MaxEnergy { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }
    public int BaseSpeedSlotCount { get; }
    public int MaxEmotionLevel { get; }      // 최대 감정 단계
    public int MaxEmotionStack { get; }      // 감정 스택 최대치 감정 스택 10 -> 감정 레벨 +1
    public int EmotionGainOnDamageDealt { get; }
    public int EmotionGainOnDamageReceived { get; }
    public int EmotionGainOnStagger { get; }
    public int EmotionGainOnStaggered { get; }
    public IReadOnlyList<PassiveData> Passives { get; }

    public CharacterData(
        string name,
        int maxHp,
        int maxStagger,
        int maxEnergy,
        int minSpeed,
        int maxSpeed,
        int baseSpeedSlotCount = 1,
        int maxEmotionLevel = 5,
        int maxEmotionStack = 10,
        int emotionGainOnDamageDealt = 0,
        int emotionGainOnDamageReceived = 0,
        int emotionGainOnStagger = 0,
        int emotionGainOnStaggered = 0,
        List<PassiveData> passives = null)
    {
        Name = name;
        MaxHp = maxHp;
        MaxStagger = maxStagger;
        MaxEnergy = maxEnergy;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
        BaseSpeedSlotCount = baseSpeedSlotCount;
        MaxEmotionLevel = maxEmotionLevel;
        MaxEmotionStack = maxEmotionStack;
        EmotionGainOnDamageDealt = emotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = emotionGainOnDamageReceived;
        EmotionGainOnStagger = emotionGainOnStagger;
        EmotionGainOnStaggered = emotionGainOnStaggered;
        Passives = passives ?? new List<PassiveData>();
    }
}