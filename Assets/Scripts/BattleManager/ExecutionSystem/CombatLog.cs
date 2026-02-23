using System.Collections.Generic;
using Unity.VisualScripting;
public class CombatLog
{
    public List<CombatEvent> Events = new();

    public void Add(CombatEvent log)
    {
        Events.Add(log);
    }
}

public class CombatEvent
{
    public CombatEventType Type;

    public SpeedSlot Source;
    public SpeedSlot Target;

    public int? SourceDiceIndex;
    public int? TargetDiceIndex;

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