public class ExileCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public ExileCardEvent(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.CardManager.ExileCard(Card);
        runtime.AddLog(new ExileCardLog(CharacterId, Card));
    }
}