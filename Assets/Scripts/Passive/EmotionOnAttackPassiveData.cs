
public class EmotionOnAttackPassiveData : PassiveData, IStatModifierPassive
{
   public int Amount { get; }

    public EmotionOnAttackPassiveData(int amount)
        : base("Emotion On Attack", PassiveType.EmotionOnAttack)
    {
        Amount = amount;
    }

    public void Apply(CharacterStateBuilder builder)
        => builder.EmotionGainOnDamageDealt += Amount;
}