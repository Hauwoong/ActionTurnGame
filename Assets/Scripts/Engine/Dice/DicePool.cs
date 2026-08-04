using System.Collections.Generic;

public class DicePool
{
    private readonly List<DiceEntry> _dice = new();

    private int _cursor;
    public void Inject(List<DiceEntry> entries) => _dice.InsertRange(_cursor, entries);

    public DiceEntry? Peek()
    {
        while (_cursor < _dice.Count)
        {
            var state = _dice[_cursor].Dice.State;
            if (state == DiceState.Ready || state == DiceState.Used || state == DiceState.Stored)
                return _dice[_cursor];
            _cursor++;
        }
        return null;
    }

    public void Advance(AdvanceType type)
    {
        if (_cursor < 0 || _cursor >= _dice.Count) return;

        var entry = _dice[_cursor];
        switch (type)
        {
            case AdvanceType.Consume:
                entry.Dice.Consume();
                _cursor++;
                break;
            case AdvanceType.Destroy:
                entry.Dice.Destroy();
                _cursor++;
                break;
            case AdvanceType.Reuse:
                entry.Dice.Use();
                break;
        }
    }
    public void EndBout() // 한 교전 끝
    {
        StoreConsumed();
        DestroyUsed();
        ClearDestroyed();
        _cursor = 0;
    }

    void StoreConsumed()
    {
        foreach (var e in _dice)
            if (e.Dice.State == DiceState.Consumed)
                e.Dice.Store();
    }

    void DestroyUsed()
    {
        foreach (var e in _dice)
        {
            if (e.Dice.State == DiceState.Used)
                e.Dice.Destroy();
        }
    }
    void ClearDestroyed() => _dice.RemoveAll(e => e.Dice.State == DiceState.Destroyed);

    public void ResetForNextTurn() // 턴 끝
    {
        _dice.Clear();
        _cursor = 0;
    }
    public void DestroyRemaining()
    {
        for (int i = _cursor; i < _dice.Count; i++)
        {
            _dice[i].Dice.Destroy();

            _cursor++;
        }
    }
    public void DiscardRemaining()
    {
        for (int i = _cursor; i < _dice.Count; i++)
        {
            var dice = _dice[i].Dice;

            if (dice.Type == DiceType.Attack)
                dice.Destroy();

            else
                dice.Consume();

             _cursor++;
        }
    }
}

public enum AdvanceType
{
    Consume,
    Destroy,
    Reuse
}