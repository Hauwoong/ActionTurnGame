public class StaggerEvent : ICombatEvent
{
    private readonly StaggerContext _ctx;

    public StaggerEvent(StaggerContext ctx)
    {
        _ctx = ctx;
    }

    public void Apply(BattleRuntime runtime)
    {
        if (_ctx.Attacker.IsDead) return;

        _ctx.Attacker.TriggerBeforeStagger(_ctx);
        _ctx.Defender.TriggerBeforeStagger(_ctx);

        if(!_ctx.IsCancelled)
        {
            if (_ctx.IsHeal)
                _ctx.Attacker.RecoverStagger(_ctx.FinalValue);
            else 
                _ctx.Defender.TakeStagger(_ctx.FinalValue);
            runtime.AddLog(new StaggerLog
            (
                _ctx.IsHeal ? _ctx.Attacker.CharacterId : _ctx.Defender.CharacterId,
                _ctx.FinalValue,
                _ctx.IsHeal
            ));
            _ctx.Attacker.TriggerAfterStagger(_ctx);
            _ctx.Defender.TriggerAfterStagger(_ctx);

            // AfterStagger 후에 흐트러짐 진입 체크
            if (!_ctx.IsHeal && _ctx.Defender.ShouldEnterStagger())
            {
                runtime.EnqueueEvent(new StaggeredEvent(_ctx.Defender.CharacterId));
            }

            if (_ctx.IsHeal)
            {
                runtime.EnqueueEvent(new GainEmotionStackEvent
                (
                    _ctx.Attacker.CharacterId,
                    _ctx.Attacker.EmotionGainOnStaggerHeal
                ));
            }
            else
            {
                runtime.EnqueueEvent(new GainEmotionStackEvent(
                    _ctx.Attacker.CharacterId,
                    _ctx.Attacker.EmotionGainOnStagger
                ));
                runtime.EnqueueEvent(new GainEmotionStackEvent(
                    _ctx.Defender.CharacterId,
                    _ctx.Defender.EmotionGainOnStaggered
                ));
            }
        }
    }
}