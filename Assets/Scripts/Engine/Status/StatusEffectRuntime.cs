
public abstract class StatusEffectRuntime
{
    private readonly int _baseDuration; public const int Permanent = -1;

    protected readonly CharacterRuntime Owner;
    public int Priority { get; } 
    public bool IsExpired { get; protected set; }
    public int Stack { get; protected set; }
    public int PendingStack { get; protected set; }
    public int Duration { get; protected set; }
    public StatusEffectType Type { get; protected set; }
    
    protected StatusEffectRuntime(CharacterRuntime owner,int stack, int duration, int priority, StatusEffectType type)
    {
        Owner = owner;
        Stack = stack;
        Duration = duration;
        _baseDuration = duration;
        Priority = priority;
        Type = type;
    }

    public virtual void AddStack(int amount, bool delayed)
    {
        if (delayed) PendingStack += amount;

        else Stack += amount;
    }
    public virtual void OnTurnStart(TurnStartContext ctx) { }
    public virtual void OnBeforeDamage(IDamageContext ctx) { }
    public virtual void OnAfterDamage(IDamageContext ctx) { }
    public virtual void OnBeforeStagger(StaggerContext ctx) { }  // 추가
    public virtual void OnAfterStagger(StaggerContext ctx) { }   // 추가
    public virtual void OnDiceRoll() { }
    protected virtual void OnTurnEnd() { }
    public void TickTurnEnd()
    {
        OnTurnEnd();

        if (Duration > 0)
        {
            Duration--;

            if (Duration <= 0)
                Stack = 0;
        }

        if (PendingStack > 0)
        {
            Stack += PendingStack;
            PendingStack = 0;
            Duration = _baseDuration;
        }

        if (Stack <= 0) IsExpired = true;
    }
    public virtual void OnModifyRoll(DiceRollContext ctx) { }
}
