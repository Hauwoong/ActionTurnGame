
public class TurnEndEvent : ICombatEvent
{
    public int CharacterId { get; }
    public TurnEndEvent(int characterId)
    {
        CharacterId = characterId;
    }
    
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.TriggerTurnEnd();
        runtime.AddLog(new TurnEndLog(CharacterId));
    }
}