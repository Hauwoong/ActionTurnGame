using System.Collections.Generic;

public abstract class CombatLog { }

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