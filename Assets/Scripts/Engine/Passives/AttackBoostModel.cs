
public class  AttackBoostModel : PassiveModel
{
    public int Amount { get; }
    public AttackBoostModel(int amount)
    {
        Amount = amount;
    }
    public override PassiveEffect CreateEffect(CharacterRuntime owner) 
        => new AttackBoostPassive(owner, Amount);
}
