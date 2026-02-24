using UnityEngine;

public class CombatEngine
{
    public static CombatLog ClashResolve(CharacterRuntime a, CharacterRuntime b)
    {
        CombatLog log = new CombatLog
        {
            UnitA = a.Owner,
            UnitB = b.Owner
        };

        while (a.ActiveQueue.HasDice && b.ActiveQueue.HasDice)
        {
            ResolveDiceVSDice(a, b, log);
        }

        if (a.ActiveQueue.HasDice)
        {
            ResolveOpposedDice(a, b, log);
        }

        if (b.ActiveQueue.HasDice)
        {
            ResolveOpposedDice(b, a, log);
        }

        return log;
    }

    static void ResolveDiceVSDice(CharacterRuntime a, CharacterRuntime b, CombatLog log)
    {
        DiceRuntime diceA = a.CurrnetDice;
        DiceRuntime diceB = b.CurrnetDice;

        DiceClashOutcome outcome = DiceEngine.Resolve(diceA, diceB);

        log.Add(new CombatEvent
        {
            UnitA = a.Owner,
            UnitB = b.Owner,
            Type = CombatEventType.DiceRolled,
            SourceRoll = outcome.RollA,
            TargetRoll = outcome.RollB
        });

        if (outcome.DestoryA)
        {
            diceA.IsDestoryed = true;
            a.ActiveQueue.DestoryDice();
        }

        if (outcome.DestoryB)
        {
            diceB.IsDestoryed = true;
            b.ActiveQueue.DestoryDice();
        }
    }

    static void ResolveOpposedDice(CharacterRuntime a, CharacterRuntime b, CombatLog log)
    {
        int originalCount = a.ActiveQueue.Count();

        for (int i = 0; i < originalCount; i++)
        {
            DiceRuntime dice = a.CurrnetDice;

            if (dice.Type != DiceType.Attack)
            {
                a.HoldQueue.PushFront(dice);
            }

            else
            {
                // 공격 주사위 로그 만들기
            }
        }

        while (a.HoldQueue.HasDice)
        {
            a.ActiveQueue.PushFront(a.HoldQueue.PopFront());
        }
    }
}
