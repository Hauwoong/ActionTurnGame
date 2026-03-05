using UnityEngine;

public abstract class StatusEffectRuntime
{
    protected readonly CharacterRuntime Owner;
    public int Priority { get; } 
    public bool IsExpird { get; protected set; }
    
    protected StatusEffectRuntime(CharacterRuntime owner, int priority)
    {
        Owner = owner;
        Priority = priority;
    }

    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnBeforeDamage(DamageContext ctx) { }
    public virtual void OnAfterDamage(DamageContext ctx) { }
}
