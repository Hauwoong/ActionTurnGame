using System.Collections.Generic;
using UnityEngine;

public class DiceQueue
{
    private LinkedList<DiceRuntime> list = new();

    public bool HasDice => list.Count > 0;

    public void PushFront(DiceRuntime dice)
    {
        list.AddFirst(dice);
    }

    public void PushBack(DiceRuntime dice)
    {
        list.AddLast(dice);
    }

    public DiceRuntime PeekFront()
    {
        return list.First.Value;
    }

    public DiceRuntime PopFront()
    {
        DiceRuntime dice = list.First.Value;
        list.RemoveFirst();
        return dice;
    }

    public int Count()
    {
        return list.Count;
    }

    public void DestoryDice()
    {
        list.RemoveFirst();
    }
}
