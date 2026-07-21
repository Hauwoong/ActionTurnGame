
public class ClashContextEvent : ICombatEvent
{
    private readonly IClashContext _ctx;

    public ClashContextEvent(IClashContext ctx)
    {
        _ctx = ctx;
    }

    public void Apply(BattleRuntime runtime)
    {
        if (_ctx.IsCancelled) return;

        switch (_ctx)
        {
            case DamageContext dmg:
                runtime.EnqueueEvent(new DamageEvent(dmg));
                break;

            case StaggerContext stagger:
                runtime.EnqueueEvent(new StaggerEvent(stagger));
                break;
        }
    }
}