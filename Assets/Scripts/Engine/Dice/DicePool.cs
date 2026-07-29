using System.Collections.Generic;

public class DicePool
{
    private readonly List<DiceEntry> _dice = new();

    private int _cursor;

    public void Add(DiceEntry entry) => _dice.Add(entry);

    public void Inject(DiceEntry entry) => _dice.Insert(_cursor, entry);

    public DiceEntry? Peek()
    {
        while (_cursor < _dice.Count)
        {
            var state = _dice[_cursor].Dice.State;
            if (state == DiceState.Ready || state == DiceState.Used)
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

    public void Recover() // 한 합 끝
    {
        foreach (var e in _dice)
            if (e.Dice.State == DiceState.Consumed)
                e.Dice.Recover();
    }

    public void DestroyUsed()
    {
        foreach (var e in _dice)
        {
            if (e.Dice.State == DiceState.Used)
                e.Dice.Destroy();
        }
    }

    public void ResetForNextTurn() // 턴 끝
    {
        _cursor = 0;
        foreach (var e in _dice)
            if (e.Dice.State != DiceState.Destroyed)
                e.Dice.Destroy();
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