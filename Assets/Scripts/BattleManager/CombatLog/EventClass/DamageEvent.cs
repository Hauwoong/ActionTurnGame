
public class DamageEvent : ICombatEvent
{
    public int CharacterId;
    public int Amount;

    public DamageEvent(int characterId, int damage)
    {
        CharacterId = characterId;
        Amount = damage;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.TakeDamage(Amount);
    }
}
