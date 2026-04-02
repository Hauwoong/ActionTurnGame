
public class DiceRecoverEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceRecoverEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.RecoverDice();
        runtime.AddLog(new DiceRecoverLog(CharacterId));
    }
}