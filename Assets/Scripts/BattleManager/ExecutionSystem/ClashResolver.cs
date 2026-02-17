using UnityEngine;

public class ClashResolver 
{
    public static void Resolve(Bout bout)
    {
        var p = bout.A.SourceSlot.owner;
        var e = bout.B.SourceSlot.owner;

        while (p.HasDice && e.HasDice)
        {
            var pd = p.PopDice();
            var ed = e.PopDice();

            ResolveDice(pd, ed);
        }
    }

    static void  ResolveDice(DiceResult P, DiceResult E)
    {
        if (P.type == DiceType.Attack && E.type == DiceType.Attack)
        {
            if (P.value > E.value)
            {
                E.owner.TakeDamage(P.value);
            }
            else if (P.value < E.value)
            {
                P.owner.TakeDamage(E.value);
            }
            else
            {
                Debug.Log("Two Player Draw!");
            }
        }

        else if (P.type == DiceType.Attack && E.type == DiceType.Block)
        {
            int dmg = P.value - E.value;

            if (dmg > 0)
            {
                E.owner.TakeDamage(dmg);
            }
        }

        else if (E.type == DiceType.Attack && P.type == DiceType.Block)
        {
            int dmg = E.value - P.value;

            if (dmg > 0)
            {
                P.owner.TakeDamage(dmg);
            }
        }
    }

    void ApplyRemainingDice(Character c, Character target)
    {
        while (c.HasDice)
        {
            var d = c.PopDice();

            if (d.type == DiceType.Attack)
            {
                target.TakeDamage(d.value);
            }
        }
    }
}
