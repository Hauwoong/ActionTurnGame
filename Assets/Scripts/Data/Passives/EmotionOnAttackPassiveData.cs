using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Emotion On Attack")]
public class EmotionOnAttackPassiveData : PassiveData
{
    [SerializeField] private int amount;
    public override PassiveModel ToModel() => new EmotionOnAttackModel(amount);
}