using System.Collections.Generic;

public class BattleInput
{
    public BoutGraph BoutGraph { get; }

    public int Seed { get; }

    public BattleInput(BoutGraph boutGraph, int seed)
    {
        BoutGraph = boutGraph;
        Seed = seed;
    }
}
