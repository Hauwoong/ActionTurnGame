public class ChangeMaxHpEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxHpEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.ChangeMaxHp(Amount);
        runtime.AddLog(new ChangeMaxHpLog(CharacterId, Amount));

        if (character.ShouldDie())
        {
            character.Die();
            runtime.EnqueueEvent(new DeathEvent(CharacterId));
        }
    }
}