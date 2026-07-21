
public class DiceDestroyUsedEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceDestroyUsedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.DestroyUsedDice();
        runtime.AddLog(new DiceDestroyUsedLog(CharacterId));
    }
}