
using UnityEditor;

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
                dmg.Attacker.TriggerBeforeDamage(dmg);
                dmg.Defender.TriggerBeforeDamage(dmg);
                if (!dmg.IsCancelled)
                {
                    dmg.Defender.TakeDamage(dmg.FinalDamage);
                    runtime.AddLog(new DamageLog(dmg.Defender.CharacterId, dmg.FinalDamage));

                    //감정 스택 추가
                    dmg.Attacker.GainEmotionStack(dmg.Attacker.EmotionGainOnDamageDealt);
                    dmg.Defender.GainEmotionStack(dmg.Defender.EmotionGainOnDamageReceived);

                    dmg.Attacker.TriggerAfterDamage(dmg);
                    dmg.Defender.TriggerAfterDamage(dmg);
                }
                break;

            case StaggerContext stagger:
                stagger.Attacker.TriggerBeforeStagger(stagger);
                stagger.Defender.TriggerBeforeStagger(stagger);
                if (!stagger.IsCancelled)
                {
                    if (stagger.IsRecover)
                        stagger.Attacker.RecoverStagger(stagger.FinalValue);
                    else
                        stagger.Defender.TakeStagger(stagger.FinalValue);
                    runtime.AddLog(new StaggerLog(
                        stagger.IsRecover ? stagger.Attacker.CharacterId : stagger.Defender.CharacterId,
                        stagger.FinalValue,
                        stagger.IsRecover
                    ));

                    // 감정 스택 추가
                    stagger.Attacker.GainEmotionStack(stagger.Attacker.EmotionGainOnStagger);
                    stagger.Defender.GainEmotionStack(stagger.Defender.EmotionGainOnStaggered);

                    stagger.Attacker.TriggerAfterStagger(stagger);
                    stagger.Defender.TriggerAfterStagger(stagger);
                }
                break;
        }
    }
}