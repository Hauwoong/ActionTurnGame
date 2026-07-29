
public class DiceDiscardRemainingEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceDiscardRemainingEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.DiscardRemainingDice();
    }
}