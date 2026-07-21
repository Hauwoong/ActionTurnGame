public class EndTurnCardEvent : ICombatEvent
{
    public int CharacterId { get; }

    public EndTurnCardEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.CardManager.EndTurn(runtime.Rng);
        runtime.AddLog(new EndTurnCardLog(CharacterId));
    }
}