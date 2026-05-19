using System;
using System.Collections.Generic;

public class ClashInputBuilder
{
    public ClashInput Build(BoutGraph graph, ActionInstance action)
    {
        if (graph.edges.TryGetValue(action.SourceSlot, out var otherSlot))
        {
            var otherAction = graph.ActionBySlot[otherSlot];

            return new ClashInput(BuildDice(action), BuildDice(otherAction));
        }
        return new ClashInput(BuildDice(action), Array.Empty<DiceEntry>());
    }

   IReadOnlyList<DiceEntry> BuildDice(ActionInstance action)
   {
        return action.Card.Dice;
   }
}
