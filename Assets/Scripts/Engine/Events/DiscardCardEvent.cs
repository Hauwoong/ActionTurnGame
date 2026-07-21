public class DiscardCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardData Card { get; }

    public DiscardCardEvent(int characterId, CardData card)
    {
        CharacterId = characterId;
        Card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.CardManager.DiscardCard(Card);
        runtime.AddLog(new DiscardCardLog(CharacterId, Card));
    }
}