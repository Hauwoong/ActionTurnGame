using UnityEngine;

public readonly struct DiceEntry
{
    public readonly DiceRuntime Dice;
    public readonly DiceHandle Handle;

    public DiceEntry(DiceRuntime dice, DiceHandle handle)
    {
        Dice = dice;
        Handle = handle;
    }
}
