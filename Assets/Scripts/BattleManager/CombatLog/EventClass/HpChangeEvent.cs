using UnityEngine;

public class HpChangeEvent
{
    public Character Target { get; }
    public int Delta { get; }

    public HpChangeEvent(Character target, int delta)
    {
        Target = target;
        Delta = delta;
    }
}
