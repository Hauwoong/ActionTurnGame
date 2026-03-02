using UnityEngine;

public class DamageEvent : ICombatEvent
{
    public CharacterRuntime Target;
    public int Amount;

    public DamageEvent(CharacterRuntime target, int damage)
    {
        Target = target;
        Amount = damage;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetRuntime(Target);
        character.TakeDamage(Amount);
    }
}
