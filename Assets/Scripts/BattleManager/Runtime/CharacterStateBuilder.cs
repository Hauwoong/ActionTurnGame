
public class CharacterStateBuilder
{
    public int MaxHp { get; set; }
    public int MaxStagger { get; set; }
    public int MaxEnergy { get; set; }
    public int EmotionGainOnDamageDealt { get; set; }
    public int EmotionGainOnDamageReceived { get; set; }
    public int EmotionGainOnStagger { get; set; }
    public int EmotionGainOnStaggered { get; set; }
    public int SpeedSlotCount { get; set; }

    public CharacterStateBuilder(CharacterData source)
    {
        MaxHp = source.MaxHp;
        MaxStagger = source.MaxStagger;
        MaxEnergy = source.MaxEnergy;
        EmotionGainOnDamageDealt = source.EmotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = source.EmotionGainOnDamageReceived;
        EmotionGainOnStagger = source.EmotionGainOnStagger;
        EmotionGainOnStaggered = source.EmotionGainOnStaggered;
        SpeedSlotCount = source.BaseSpeedSlotCount;
    }
}