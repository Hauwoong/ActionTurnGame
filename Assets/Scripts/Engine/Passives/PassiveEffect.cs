
public abstract class PassiveEffect
{
    protected readonly CharacterRuntime Owner;
    public PassiveType Type { get; }

    protected PassiveEffect(CharacterRuntime owner, PassiveType type)
    {
        Owner = owner;
        Type = type;
    }

    public virtual void OnBeforeDamage(IDamageContext ctx) { }
    public virtual void OnAfterDamage(IDamageContext ctx) { }
    public virtual void OnBeforeStagger(StaggerContext ctx) { }
    public virtual void OnAfterStagger(StaggerContext ctx) { }
    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnTurnEnd() { }
    public virtual void OnModifyRoll(DiceRollContext ctx) { }

}