public class DiscardCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public DiscardCardEvent(int characterId, CardModel card)
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