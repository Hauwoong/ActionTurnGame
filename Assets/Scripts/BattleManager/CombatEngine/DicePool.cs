using System.Collections.Generic;

public class DicePool
{
    private readonly List<DiceRuntime> _dice = new();

    private int _cursor;

    public void Add(DiceRuntime dice)
    {
        _dice.Add(dice);
    }

    public void Inject(DiceRuntime dice)
    {
        _dice.Insert(_cursor, dice);
    }

    public DiceRuntime Peek()
    {
        while (_cursor < _dice.Count)
        {
            if (_dice[_cursor].State == DiceState.Ready)
                return _dice[_cursor];
            _cursor++;
        }
        return null;
    }

    public void Advance(AdvanceType type)
    {
        var dice = _dice[_cursor];
        switch (type)
        {
            case AdvanceType.Consume:
                dice.Consume();
                _cursor++;
                break;

            case AdvanceType.Destroy:
                dice.Destroy();
                _cursor++;
                break;

            case AdvanceType.Reuse:
                break;
        }
    }

    public void Recover() // 한 합 끝
    {
        foreach (var d in _dice)
        {
            d.Recover();
        }
    }

    public void ResetForNextTurn() // 턴 끝
    {
        _cursor = 0;
        foreach (var d in _dice)
        {
            if (d.State != DiceState.Destroyed)
                d.Destroy();
        }
    }
}

public enum AdvanceType
{
    Consume,
    Destroy,
    Reuse
}