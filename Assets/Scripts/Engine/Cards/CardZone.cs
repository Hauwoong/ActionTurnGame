using System.Collections.Generic;

public class CardZone
{
    private readonly List<CardModel> _cards = new();
    public IReadOnlyList<CardModel> Cards => _cards;
    public int Count => _cards.Count;

    public void Add(CardModel card) => _cards.Add(card);
    public void Remove(CardModel card) => _cards.Remove(card);

    public CardModel DrawTop()
    {
        if (_cards.Count == 0) return null;
        var card = _cards[0];
        _cards.RemoveAt(0);
        return card;
    }

    public void AddRange(List<CardModel> cards) => _cards.AddRange(cards);

    public List<CardModel> TakeAll()
    {
        var cards = new List<CardModel>(_cards);
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