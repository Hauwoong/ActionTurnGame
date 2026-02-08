
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpeedSlot
{
    public Character owner;
    public int index;

    // 굴림 속도
    public int speed => owner.rolledSpeeds[index];

    // 선택한 카드
    public CardData card;

    // 타겟 슬롯
    public SpeedSlot target;

    // 인터셉트 후보들
    public List<SpeedSlot> interceptCandidates = new();

    // 현재 선택된 합 상대
    public SpeedSlot currentBout;

    // 슬롯 사용 유무
    public bool IsUsed => owner.IsSlotUsed(index);

    public void Use()
    {
        owner.UseSlot(index);
    }

    public void Clear()
    {
        card = null;
        target = null;
        interceptCandidates.Clear();
        currentBout = null;
    }
}
   

