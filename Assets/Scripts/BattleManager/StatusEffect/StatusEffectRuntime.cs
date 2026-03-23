
public abstract class StatusEffectRuntime
{
    protected readonly CharacterRuntime Owner;
    public int Priority { get; } 
    public bool IsExpired { get; protected set; }
    public int Stack { get; protected set; }
    public int Duration { get; protected set; }
    
    protected StatusEffectRuntime(CharacterRuntime owner,int stack, int priority)
    {
        Owner = owner;
        Stack = stack;
        Priority = priority;
    }

    public virtual void AddStack(int amount) => Stack += amount;
    public virtual void ReduceStack(int amount) => Stack -= amount;
    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnBeforeDamage(DamageContext ctx) { }
    public virtual void OnAfterDamage(DamageContext ctx) { }
    public virtual void OnDiceClash() { }
    public virtual void OnTurnEnd()
    {
        Duration--;

        if (Duration <= 0)
        {
            IsExpired = true;
        }
    }
}
