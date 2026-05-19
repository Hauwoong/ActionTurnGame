
public abstract class CombatLog { }

public enum CombatEventType
{
    DiceRolled,
    DiceDestroyed,
    ClashStarted,
    ClashEnded,
    UnopposedAttack,
    DamageApplied,
    CombatEnd
}