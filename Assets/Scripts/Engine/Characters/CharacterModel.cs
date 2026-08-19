using System.Collections.Generic;
public class CharacterModel
{
    public string Name { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int MaxEnergy { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }
    public int BaseSpeedSlotCount { get; }
    public int MaxEmotionLevel { get; }
    public int MaxEmotionStack { get; }
    public int EmotionGainOnDamageDealt { get; }
    public int EmotionGainOnDamageReceived { get; }
    public int EmotionGainOnStagger { get; }
    public int EmotionGainOnStaggered { get; }
    public int EmotionGainOnStaggerHeal { get; }
    public IReadOnlyList<PassiveModel> Passives { get; }
    public IReadOnlyList<CardModel> InitialDeck { get; }
    public CharacterModel(
        string name,
        int maxHp,
        int maxStagger,
        int maxEnergy,
        int minSpeed,
        int maxSpeed,
        int baseSpeedSlotCount,
        int maxEmotionLevel,
        int maxEmotionStack,
        int emotionGainOnDamageDealt,
        int emotionGainOnDamageReceived,
        int emotionGainOnStagger,
        int emotionGainOnStaggered,
        int emotionGainOnStaggerHeal,
        IReadOnlyList<PassiveModel> passives,
        IReadOnlyList<CardModel> initialDeck)
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
        EmotionGainOnStaggerHeal = emotionGainOnStaggerHeal;
        Passives = passives;
        InitialDeck = initialDeck;
    }
}