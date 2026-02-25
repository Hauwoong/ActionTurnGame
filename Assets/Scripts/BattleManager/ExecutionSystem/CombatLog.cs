using System.Collections.Generic;

public class CombatLog
{
    public Character UnitA;
    public Character UnitB;

    public List<CombatEvent> Events = new();

    public void Add(CombatEvent log)
    {
        Events.Add(log);
    }
}

public class CombatEvent
{
    public Character UnitA;
    public Character UnitB;

    public CombatEventType Type;

    public int? SourceRoll;
    public int? TargetRoll;

    public int? Damage;
}

public enum CombatEventType
{
    DiceRolled,
    DiceDestoryed,
    ClashStarted,
    ClashEnded,
    UnopposedAttack,
    DamageApplied,
    CombatEnd
}