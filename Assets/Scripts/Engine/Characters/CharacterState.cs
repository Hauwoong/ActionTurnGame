using System.Collections.Generic;

// 불변 스냅샷 계층. CharacterModel(순수 청사진)에 스탯 수정 패시브를 적용해 한 번 얼리고.
// CharacterRuntime(가변)이 시작값으로 읽는다. 패시브 적용은 CharacterStateBuilder가 맡는다.
public sealed class CharacterState
{
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
    public IReadOnlyList<PassiveModel> Passives { get; }
    public IReadOnlyList<CardModel> InitialDeck { get; }

    /// <summary>
    /// 청사진(CharacterModel)를 불변 스냅샷으로 굳히는 변환 관문
    /// </summary>
    /// <param name="source">변환할 캐릭터 청사진</param>
    /// <param name="id">전투 전역 캐릭터 번호</param>
    /// <param name="team">소속 진영(Ally/Enemy)</param>
    public CharacterState(CharacterModel source, int id, Team team)
    {
        CharacterId = id;
        Team = team;

        // builder로 자원 수정 패시브 적용
        var builder = new CharacterStateBuilder(source);
        foreach (var passive in source.Passives)
        {
            if (passive is IStatModifierPassive statModifier)
                statModifier.Apply(builder);
        }

        // builder.x = 스탯 수정 패시브(IStatModifierPassive)가 건드릴 수 있는 값 -> builder 거쳐 보정된 결과.
        // source.x (MinSpeed/MaxSpeed/MaxEmotionStack/MaxEmotionLevel) = 패시브 영향 밖이라 청사진에서 그대로 복사.
        MaxHp = builder.MaxHp;
        MaxStagger = builder.MaxStagger;
        MaxEnergy = builder.MaxEnergy;
        SpeedSlotCount = builder.SpeedSlotCount;
        MinSpeed = source.MinSpeed; //아직 builder 에 필드가 없어 청사진 값 그대로. 속도 패시브를 만들려면 builder 에 필드 추가 + 이 줄을 builder.X 로 바꿔야 함.
        MaxSpeed = source.MaxSpeed; //아직 builder 에 필드가 없어 청사진 값 그대로. 속도 패시브를 만들려면 builder 에 필드 추가 + 이 줄을 builder.X 로 바꿔야 함.
        MaxEmotionStack = source.MaxEmotionStack;
        MaxEmotionLevel = source.MaxEmotionLevel;
        EmotionGainOnDamageDealt = builder.EmotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = builder.EmotionGainOnDamageReceived;
        EmotionGainOnStagger = builder.EmotionGainOnStagger;
        EmotionGainOnStaggered = builder.EmotionGainOnStaggered;
        EmotionGainOnStaggerHeal = builder.EmotionGainOnStaggerHeal;
        Passives = source.Passives;
        InitialDeck = source.InitialDeck;
    }
}