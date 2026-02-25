using System.Collections.Generic;
public class CombatEngine
{
    public static CombatLog ClashResolve(CharacterRuntime a, CharacterRuntime b)
    {
        CombatLog log = new CombatLog
        {
            UnitA = a.Owner,
            UnitB = b.Owner
        };

        while (!a.IsFinished && !b.IsFinished)
        {
            ResolveDiceVSDice(a, b, log);
        }

        if (a.IsFinished)
        {
            ResolveOpposedDice(b, a, log);
        }

        if (b.IsFinished)
        {
            ResolveOpposedDice(a, b, log);
        }    
        
        a.CleanupAfterAction();
        b.CleanupAfterAction();

        return log;
    }

    static void ResolveDiceVSDice(CharacterRuntime a, CharacterRuntime b, CombatLog log)
    {
        DiceRuntime diceA = a.CurrentDice;
        DiceRuntime diceB = b.CurrentDice;

        DiceClashOutcome outcome = DiceEngine.ResolveClash(diceA, diceB);

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
            a.Advance();
        }

        if (outcome.DestoryB)
        {
            diceB.IsDestoryed = true;
            b.Advance();
        }
    }

    static void ResolveOpposedDice(CharacterRuntime a, CharacterRuntime b, CombatLog log)
    {
        while (!a.IsFinished)
        {
            DiceRuntime dice = a.CurrentDice;

            if (dice.Type != DiceType.Attack)
            {
                a.SkipDice();
                continue;
            }

            DiceUnopposedOutcome outcome = DiceEngine.ResolveUnopposed(dice);
        }
    }
}
