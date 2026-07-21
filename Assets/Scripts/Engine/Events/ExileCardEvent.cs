public class ExileCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardData Card { get; }

    public ExileCardEvent(int characterId, CardData card)
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