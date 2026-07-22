public class DrawCardLog : CombatLog
{
    public int CharacterId { get; }
    public int Count { get; }

    public DrawCardLog(int characterId, int count)
    {
        CharacterId = characterId;
        Count = count;
    }
}

public class UseCardLog : CombatLog
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public UseCardLog(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }
}

public class DiscardCardLog : CombatLog
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public DiscardCardLog(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }
}

public class ExileCardLog : CombatLog
{
    public int CharacterId { get; }
    public CardModel Card { get; }

    public ExileCardLog(int characterId, CardModel card)
    {
        CharacterId = characterId;
        Card = card;
    }
}

public class EndTurnCardLog : CombatLog
{
    public int CharacterId { get; }

    public EndTurnCardLog(int characterId)
    {
        CharacterId = characterId;
    }
}