public class StaggeredEvent : ICombatEvent
{
    public int CharacterId { get; }

    public StaggeredEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.EnterStagger();
        runtime.AddLog(new StaggeredLog(CharacterId));
    }
}