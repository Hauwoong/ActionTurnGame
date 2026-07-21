public class ChangeMaxStaggerEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxStaggerEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.ChangeMaxStagger(Amount);
        runtime.AddLog(new ChangeMaxStaggerLog(CharacterId, Amount));

        if (character.ShouldEnterStagger())
        {
            runtime.EnqueueEvent(new StaggeredEvent(CharacterId));
        }
    }
}