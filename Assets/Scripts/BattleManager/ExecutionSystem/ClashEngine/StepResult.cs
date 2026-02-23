using System;
using System.Collections.Generic;

public class ClashResult
{
    public SpeedSlot Attacker;
    public SpeedSlot Defender;

    public List<ClashStepResult> Steps = new();

    public bool AttackerWon;
    public bool DefenderWon;

    public int AttackerRemainingDice;
    public int DefenderRemainingDice;
}

public class ClashStepResult
{
    public int AttackerDiceIndex;
    public int DefenderDiceIndex;

    public int AttackerDiceRoll;
    public int DefenderDiceRoll;

    public ClashStepOutcome Outcome;
}

public struct DiceClashOutcome
{
    public int RollA;
    public int RollB;

    public bool DestoryA;
    public bool DestoryB;
}

public enum ClashStepOutcome
{
    AttackerWin,
    DefenderWin,
    Draw
}