using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class HpChangeEvent : ICombatEvnet
{
    public Character Target { get; private set; }
    public int Delta { get; private set; }

    public HpChangeEvent(Character target, int delta)
    {
        Target = target;
        Delta = delta;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacter(Target);
    }
}
