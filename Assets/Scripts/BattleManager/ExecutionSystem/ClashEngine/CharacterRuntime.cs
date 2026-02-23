using System.Collections.Generic;

public class CharacterRuntime
{
    public SpeedSlot slot;

    private Queue<DiceRuntime> defencestack = new();

    public bool HasDefenceDice => defencestack.Count > 0;

    public DiceRuntime PopDice()
    {
        return defencestack.Count > 0 ? defencestack.Dequeue() : null;
    }

    public void PushDice(DiceRuntime dice)
    {
        defencestack.Enqueue(dice);
    }

    public void Clear()
    {
        defencestack.Clear();
    }
}
