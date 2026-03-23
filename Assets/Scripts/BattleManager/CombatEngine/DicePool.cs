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
            if (_dice[_cursor].Dice.State == DiceState.Ready)
                return _dice[_cursor];
            _cursor++;
        }
        return null;
    }

    public void Advance(AdvanceType type)
    {
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
                break;
        }
    }

    public void Recover() // 한 합 끝
    {
        foreach (var e in _dice)
            if (e.Dice.State == DiceState.Consumed)
                e.Dice.Recover();
    }

    public void ResetForNextTurn() // 턴 끝
    {
        _cursor = 0;
        foreach (var e in _dice)
            if (e.Dice.State != DiceState.Destroyed)
                e.Dice.Destroy();
    }
}

public enum AdvanceType
{
    Consume,
    Destroy,
    Reuse
}