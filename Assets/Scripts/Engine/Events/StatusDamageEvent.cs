public class StatusDamageEvent : ICombatEvent
{
    private readonly StatusDamageContext _ctx;

    public StatusDamageEvent(StatusDamageContext ctx)
    {
        _ctx = ctx;
    }

    public void Apply(BattleRuntime runtime)
    {
        _ctx.Defender.TriggerBeforeDamage(_ctx);

        if (!_ctx.IsCancelled)
        {
            runtime.AddLog(new StatusDamageLog(_ctx.Defender.CharacterId, _ctx.FinalDamage));
            _ctx.Defender.TakeDamage(_ctx.FinalDamage);
            _ctx.Defender.TriggerAfterDamage(_ctx);

            if (_ctx.Defender.ShouldDie())
            {
                runtime.EnqueueEvent(new DeathEvent(_ctx.Defender.CharacterId));
            }
        }
    }
}