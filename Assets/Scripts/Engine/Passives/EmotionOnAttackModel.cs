
public class EmotionOnAttackModel : PassiveModel, IStatModifierPassive
{
    public int Amount { get; }
    public EmotionOnAttackModel(int amount)
    {
        Amount = amount;
    }
    public override PassiveEffect CreateEffect(CharacterRuntime owner)
        => null;
    public void Apply(CharacterStateBuilder builder) => builder.EmotionGainOnDamageDealt += Amount;
}
