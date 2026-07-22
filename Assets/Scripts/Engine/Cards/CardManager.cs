using System.Collections.Generic;

public class CardManager
{
    private readonly CardZone _deck = new();
    private readonly CardZone _hand = new();
    private readonly CardZone _used = new();
    private readonly CardZone _discard = new();
    private readonly CardZone _exile = new();

    public IReadOnlyList<CardModel> Deck => _deck.Cards;
    public IReadOnlyList<CardModel> Hand => _hand.Cards;
    public IReadOnlyList<CardModel> Used => _used.Cards;
    public IReadOnlyList<CardModel> Discard => _discard.Cards;
    public IReadOnlyList<CardModel> Exile => _exile.Cards;

    public CardManager(List<CardModel> initialDeck, IRng rng)
    {
        _deck.AddRange(initialDeck);
        _deck.Shuffle(rng);
    }

    public CardModel Draw(IRng rng)
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

    public void UseCard(CardModel card)
    {
        _hand.Remove(card);
        _used.Add(card);
    }

    public void DiscardCard(CardModel card)
    {
        _hand.Remove(card);
        _discard.Add(card);
    }

    public void ExileCard(CardModel card)
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