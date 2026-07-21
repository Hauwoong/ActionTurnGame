public class UseCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardData Card { get; }

    public UseCardEvent(int characterId, CardData card)
    {
        CharacterId = characterId;
        Card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.CardManager.UseCard(Card);
        runtime.AddLog(new UseCardLog(CharacterId, Card));
    }
}