using UnityEngine;

public class CombatEngine
{
    public static CombatLog ClashResolve(ActionRuntime a, ActionRuntime b)
    {
        CombatLog log = new();

        log.Add(new CombatEvent
        {
            Source = a.Source.SourceSlot,
            Target = b.Source.SourceSlot,
            Type = CombatEventType.ClashStarted
        });

        while (!a.IsFinished && !b.IsFinished)
        {
            ResolveDiceVSDice(a, b, log);
        }

        
    }

    static void ContinueUnopposedResolve(Character attacker, ActionRuntime Defender, CombatLog log)
    {

    }

    static void ResolveDiceVSDice(ActionRuntime a, ActionRuntime b, CombatLog log)
    {
        DiceRuntime diceA = a.CurrentDice;
        DiceRuntime diceB = b.CurrentDice;

        DiceClashOutcome outcome = DiceEngine.Resolve(diceA, diceB);

        log.Add(new CombatEvent
        {
            Source = a.Source.SourceSlot,
            Target = b.Source.SourceSlot,
            Type = CombatEventType.DiceRolled,
            SourceDiceIndex = a.CurrentIndex,
            TargetDiceIndex = b.CurrentIndex,
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
}
