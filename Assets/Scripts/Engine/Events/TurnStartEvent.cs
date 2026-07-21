
public class TurnStartEvent : ICombatEvent
{
    public int CharacterId { get; }
    public TurnStartEvent(int characterId)
    {
        CharacterId = characterId;
    }
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.TriggerTurnStart();
        runtime.AddLog(new TurnStartLog(CharacterId));
    }
}