
public class ReturnCardLog : CombatLog
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public ReturnCardLog(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }
}