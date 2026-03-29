
public class DamageEvent : ICombatEvent
{
    public int AttackerId { get; }
    public int DefenderId { get; }
    public int BaseDamage { get; }

    public DamageEvent(int attackerId, int defenderId, int baseDamage)
    {
        AttackerId = attackerId;
        DefenderId = defenderId;
        BaseDamage = baseDamage;
    }

    public void Apply(BattleRuntime runtime)
    {
        var attacker = runtime.GetCharacterRuntime(AttackerId);
        var defender = runtime.GetCharacterRuntime(DefenderId);

        var ctx = new DamageContext(attacker, defender, BaseDamage);

        // 공격자 상태이상 먼저 (힘, 마비 등 데미지 수정)
        attacker.TriggerBeforeDamage(ctx);
        // 방어자 상태이상 (데미지 감소 등)
        defender.TriggerBeforeDamage(ctx);

        if (!ctx.IsCancelled)
        {
            defender.TakeDamage(ctx.FinalDamage);
            runtime.AddLog(new DamageLog(DefenderId, ctx.FinalDamage));

            attacker.TriggerAfterDamage(ctx);
            defender.TriggerAfterDamage(ctx);
        }
    }
}
