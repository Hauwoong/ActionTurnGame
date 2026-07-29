public class DeathEvent : ICombatEvent
{
    public int CharacterId { get; }
    public DeathEvent(int characterId)
    {
        CharacterId = characterId;
    }
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);

        character.Die();

        character.DestroyRemainingDice();

        runtime.AddLog(new DeathLog(CharacterId));

        if (runtime.TryGetBattleResult(out var winner))
            runtime.EnqueueEvent(new BattleEndEvent(winner));
    }
}