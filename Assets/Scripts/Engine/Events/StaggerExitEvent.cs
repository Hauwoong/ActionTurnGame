public class StaggerExitEvent : ICombatEvent
{
    public int CharacterId { get; }

    public StaggerExitEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.ExitStagger();
        runtime.AddLog(new StaggerExitLog(CharacterId));
    }
}