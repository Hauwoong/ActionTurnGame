using System.Collections.Generic;

public class CardZone
{
    private readonly List<CardData> _cards = new();
    public IReadOnlyList<CardData> Cards => _cards;
    public int Count => _cards.Count;

    public void Add(CardData card) => _cards.Add(card);

    public void Remove(CardData card) => _cards.Remove(card);

    public CardData DrawTop()
    {
        if (_cards.Count == 0) return null;
        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void AddRange(List<CardData> cards) => _cards.AddRange(cards);

    public List<CardData> TakeAll()
    {
        var cards = new List<CardData>(_cards);
        _cards.Clear();
        return cards;
    }

    public void Shuffle(IRng rng)
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = rng.Range(0, i);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }
}