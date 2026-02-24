using System.Collections.Generic;

public class CharacterRuntime
{
    public Character Owner;

    public DiceQueue ActiveQueue = new DiceQueue();

    public DiceQueue HoldQueue = new DiceQueue();

    public DiceRuntime CurrnetDice => ActiveQueue.HasDice ? ActiveQueue.PeekFront() : null;
}
