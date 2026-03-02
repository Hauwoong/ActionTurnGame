using UnityEngine;

public class DeathEvent
{
    public Character Target { get; }

    public DeathEvent(Character target)
    {
        Target = target;
    }
}
