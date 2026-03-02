using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BattleResolutionLoop
{
    private readonly BattleState state;

    private readonly Queue<DamageEvent> damageQueue = new();
    private readonly Queue<HpChangeEvent> hpQueue = new();
    private readonly Queue<DeathEvent> deathQueue = new();

    private const int MAX_CHAIN = 20;

    public BattleResolutionLoop(BattleState state)
    {
        this.state = state;
    }

    public void EnqueueDamage(DamageEvent damage)
    {
        damageQueue.Enqueue(damage);
    }

    public void Resolve()
    {
        int chainGuard = 0;

        while (damageQueue.Count > 0)
        {
            if (chainGuard++ > MAX_CHAIN) break;

            ResolveDamagePhase();
            ApplyHpPhase();
            DeathCheckPhase();
            ResolveDeathPhase();
        }
    }

    private void ResolveDamagePhase()
    {
        int count = damageQueue.Count;

        for (int i = 0; i < count; i++)
        {
            var damage = damageQueue.Dequeue();

            if (damage.Target.IsDead) continue;

            hpQueue.Enqueue(new HpChangeEvent(damage.Target, -damage.FinalDamage));
        }
    }

    private void ApplyHpPhase()
    {
        while (hpQueue.Count > 0)
        {
            var hp = hpQueue.Dequeue();

            if (hp.Target.IsDead) continue;

            hp.Target.Hp += hp.Delta;
        }
    }

    private void DeathCheckPhase()
    {
        foreach (var unit in state.Units)
        {
            if (!unit.IsDead && unit.currentHP <= 0)
            {
                deathQueue.Enqueue(new DeathEvent(unit));
            }
        }
    }

    private void ResolveDeathPhase()
    {
        while (deathQueue.Count > 0)
        {
            var death = deathQueue.Dequeue();

            if (death.Target.IsDead) continue;

            death.Target.IsDead = true;
        }
    }
}
