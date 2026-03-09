using System.Collections.Generic;

public class BattleInput
{
    public IReadOnlyDictionary<SpeedSlot, ActionInstance> ActionBySlot { get; }
    public BoutGraph BoutGraph { get; }

    public BattleInput(
        IReadOnlyDictionary<SpeedSlot, ActionInstance> actionBySlot, BoutGraph boutGraph)
    {
        ActionBySlot = actionBySlot;
        BoutGraph = boutGraph;
    }
}
