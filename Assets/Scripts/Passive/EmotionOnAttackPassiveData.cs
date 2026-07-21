using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Emotion On Attack")]
public class EmotionOnAttackPassiveData : PassiveData, IStatModifierPassive
{
    [SerializeField] private int amount;
    public override PassiveType Type => PassiveType.EmotionOnAttack;
    public int Amount => amount;
    public void Apply(CharacterStateBuilder builder)
        => builder.EmotionGainOnDamageDealt += Amount;
}