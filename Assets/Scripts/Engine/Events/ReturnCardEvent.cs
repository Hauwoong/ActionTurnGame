
public class ReturnCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public ReturnCardEvent(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.CardManager.ReturnCard(Card);
        runtime.AddLog(new ReturnCardLog(CharacterId, Card));
    }
}