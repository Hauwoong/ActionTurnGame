
public abstract class StatusEffectRuntime
{
    protected readonly CharacterRuntime Owner;
    public int Priority { get; } 
    public bool IsExpired { get; protected set; }
    public int Stack { get; protected set; }
    public int Duration { get; protected set; }
    public StatusEffectType Type { get; protected set; }
    
    protected StatusEffectRuntime(CharacterRuntime owner,int stack, int priority, StatusEffectType type)
    {
        Owner = owner;
        Stack = stack;
        Priority = priority;
        Type = type;
    }

    public virtual void AddStack(int amount) => Stack += amount;
    public virtual void ReduceStack(int amount) => Stack -= amount;
    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnBeforeDamage(IDamageContext ctx) { }
    public virtual void OnAfterDamage(IDamageContext ctx) { }
    public virtual void OnBeforeStagger(StaggerContext ctx) { }  // 추가
    public virtual void OnAfterStagger(StaggerContext ctx) { }   // 추가
    public virtual void OnBeforeClash(ClashContext ctx, bool IsOwnerA) { } 
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
