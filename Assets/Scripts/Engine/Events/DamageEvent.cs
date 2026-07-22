
using System.Runtime.InteropServices;

public class DamageEvent : ICombatEvent
{
    private readonly DamageContext _ctx;
    public DamageEvent(DamageContext ctx)
    {
        _ctx = ctx;
    }

    public void Apply(BattleRuntime runtime)
    {
        _ctx.Attacker.TriggerBeforeDamage(_ctx);
        _ctx.Defender.TriggerBeforeDamage(_ctx);

        if (!_ctx.IsCancelled)
        {
            _ctx.Defender.TakeDamage(_ctx.FinalDamage);
            runtime.AddLog(new DamageLog(_ctx.Defender.CharacterId, _ctx.FinalDamage));

            _ctx.Attacker.TriggerAfterDamage(_ctx);
            _ctx.Defender.TriggerAfterDamage(_ctx);

            // AfterDamage 후에 사망 체크
            if (_ctx.Defender.ShouldDie())
            {
                runtime.EnqueueEvent(new DeathEvent(_ctx.Defender.CharacterId));
            }

            // 감정 스택
            runtime.EnqueueEvent(new GainEmotionStackEvent(_ctx.Attacker.CharacterId, _ctx.Attacker.EmotionGainOnDamageDealt));
            runtime.EnqueueEvent(new GainEmotionStackEvent(_ctx.Defender.CharacterId, _ctx.Defender.EmotionGainOnDamageReceived));
        }
    }
}
