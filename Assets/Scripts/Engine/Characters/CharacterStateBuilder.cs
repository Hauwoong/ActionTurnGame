
// 스탯 수정 패시브가 값을 보정하는 임시 작업대. CharacterState는 불변이라 여기서 set로 받아 패시브 적용 후 얼린다.
public class CharacterStateBuilder
{
    public int MaxHp { get; set; }
    public int MaxStagger { get; set; }
    public int MaxEnergy { get; set; }
    public int EmotionGainOnDamageDealt { get; set; }
    public int EmotionGainOnDamageReceived { get; set; }
    public int EmotionGainOnStagger { get; set; }
    public int EmotionGainOnStaggered { get; set; }
    public int EmotionGainOnStaggerHeal { get; set; }
    public int SpeedSlotCount { get; set; }

    /// <summary>
    /// 청사진 값을 작업대에 복사해 패시브 적용 전 초기값으로 세팅한다
    /// </summary>
    /// <param name="source">기준이 될 캐릭터 청사진</param>
    public CharacterStateBuilder(CharacterData source)
    {
        MaxHp = source.MaxHp;
        MaxStagger = source.MaxStagger;
        MaxEnergy = source.MaxEnergy;
        EmotionGainOnDamageDealt = source.EmotionGainOnDamageDealt;
        EmotionGainOnDamageReceived = source.EmotionGainOnDamageReceived;
        EmotionGainOnStagger = source.EmotionGainOnStagger;
        EmotionGainOnStaggered = source.EmotionGainOnStaggered;
        EmotionGainOnStaggerHeal = source.EmotionGainOnStaggerHeal;
        SpeedSlotCount = source.BaseSpeedSlotCount; // 청사진의 기본 슬롯 수 -> 패시브가 여기서 증감 -> 최종 SpeedSlotCount
    }
}