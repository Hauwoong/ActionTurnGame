using System.Collections.Generic;

public sealed class CharacterState
{
    public CharacterData Source { get; }
    public int CharacterId { get; }
    public Team Team { get; }
    public int MaxHp { get; }
    public int MaxStagger { get; }
    public int MaxEnergy { get; }
    public int SpeedSlotCount { get; }
    public int MinSpeed { get; }
    public int MaxSpeed { get; }
    public int MaxEmotionStack { get; }
    public int MaxEmotionLevel { get; }
    public int EmotionGainOnDamageDealt { get; }
    public int EmotionGainOnDamageReceived { get; }
    public int EmotionGainOnStagger { get; }
    public int EmotionGainOnStaggered { get; }
    public int EmotionGainOnStaggerHeal { get; }
    public IReadOnlyList<PassiveData> Passives { get; }
    public IReadOnlyList<CardData> InitialDeck { get; }

    public CharacterState(CharacterData source, int id, Team team)
    {
        Source = source;
        CharacterId = id;
        Team = team;

        // builder로 자원 수정 패시브 적용
        var builder = new CharacterStateBuilder(source);
        foreach (var passive in source.Passives)
        {
            if (passive is IStatModifierPassive statModifier)
                statModifier.Apply(builder);
        }

        // builder 값으로 초기화
        MaxHp = builder.MaxHp;
        MaxStagger = builder.MaxStagger;
        MaxEnergy = builder.MaxEnergy;
        SpeedSlotCount = builder.SpeedSlotCount;
        MinSpeed = source.MinSpeed;
        MaxSpeed = source.MaxSpeed;
        MaxEmotionStack = source.MaxEmotionStack;
        MaxEmotionLevel = source.MaxEmotionLevel;
        EmotionGainOnDamageDealt = builder.EmotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = builder.EmotionGainOnDamageReceived;
        EmotionGainOnStagger = builder.EmotionGainOnStagger;
        EmotionGainOnStaggered = builder.EmotionGainOnStaggered;
        EmotionGainOnStaggerHeal = builder.EmotionGainOnStaggerHeal;
        Passives = new List<PassiveData>(source.Passives);
        InitialDeck = new List<CardData>(source.InitialDeck);
    }
}