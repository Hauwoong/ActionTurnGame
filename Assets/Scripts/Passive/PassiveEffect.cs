
public abstract class PassiveEffect
{
    protected readonly CharacterRuntime Owner;
    public PassiveType Type { get; }

    protected PassiveEffect(CharacterRuntime owner, PassiveType type)
    {
        Owner = owner;
        Type = type;
    }

    public virtual void OnBeforeClash(ClashContext ctx, bool isOwnerA) { }
    public virtual void OnBeforeDamage(DamageContext ctx) { }
    public virtual void OnBeforeStagger(StaggerContext ctx) { }
    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnTurnEnd() { }

}