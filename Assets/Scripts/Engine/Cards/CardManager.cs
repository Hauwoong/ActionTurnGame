using System.Collections.Generic;

public class CardManager
{
    private readonly CardZone _deck = new();
    private readonly CardZone _hand = new();
    private readonly CardZone _used = new();
    private readonly CardZone _discard = new();
    private readonly CardZone _exile = new();

    public IReadOnlyList<CardData> Deck => _deck.Cards;
    public IReadOnlyList<CardData> Hand => _hand.Cards;
    public IReadOnlyList<CardData> Used => _used.Cards;
    public IReadOnlyList<CardData> Discard => _discard.Cards;
    public IReadOnlyList<CardData> Exile => _exile.Cards;

    public CardManager(List<CardData> initialDeck, IRng rng)
    {
        _deck.AddRange(initialDeck);
        _deck.Shuffle(rng);
    }

    public CardData Draw(IRng rng)
    {
        if (_deck.Count == 0 && _discard.Count > 0)
        {
            _deck.AddRange(_discard.TakeAll());
            _deck.Shuffle(rng);
        }

        if (_deck.Count == 0) return null; // No cards to draw

        var card = _deck.DrawTop();
        if (card != null)
            _hand.Add(card);
        return card;
    }

    public int DrawMultiple(int count, IRng rng)
    {
        int drawn = 0;
        for (int i = 0; i < count; i++)
        {
            var card = Draw(rng);
            if (card == null) break; // 더 이상 뽑을 수 없음
            drawn++;
        }
        return drawn; // 실제로 뽑은 수 반환
    }

    public void UseCard(CardData card)
    {
        _hand.Remove(card);
        _used.Add(card);
    }

    public void DiscardCard(CardData card)
    {
        _hand.Remove(card);
        _discard.Add(card);
    }

    public void ExileCard(CardData card)
    {
        _hand.Remove(card);
        _exile.Add(card);
    }

    public void EndTurn(IRng rng)
    {
        _deck.AddRange(_used.TakeAll());
        _deck.AddRange(_discard.TakeAll());
        _deck.Shuffle(rng);
    }
}