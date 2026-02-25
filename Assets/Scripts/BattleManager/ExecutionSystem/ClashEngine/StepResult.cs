using System;
using System.Collections.Generic;

public struct DiceClashOutcome
{
    public int RollA;
    public int RollB;

    public bool DestoryA;
    public bool DestoryB;
}

public struct DiceUnopposedOutcome
{
    public int RollA;

    public bool DestoryA;

    public int Damage;
}