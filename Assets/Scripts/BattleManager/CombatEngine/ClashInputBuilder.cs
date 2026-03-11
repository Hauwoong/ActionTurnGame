<<<<<<< HEAD
<<<<<<< HEAD
using System;
using System.Collections.Generic;

=======
>>>>>>> d69ce474b7714212b5ec697ad90a7fa13dafd4dc
=======
>>>>>>> d69ce474b7714212b5ec697ad90a7fa13dafd4dc
public class ClashInputBuilder
{
    public ClashInput Build(BoutGraph graph, ActionInstance action)
    {
<<<<<<< HEAD
<<<<<<< HEAD
        if (graph.edges.TryGetValue(action.SourceSlot, out var otherSlot))
        {
            var otherAction = graph.ActionBySlot[otherSlot];

            return new ClashInput(BuildDice(action), BuildDice(otherAction));
        }
        return new ClashInput(BuildDice(action), Array.Empty<DiceEntry>()); // ÀÌ°Ç ¾î¶»°Ô °íÃÄ¾ß ÁÁÀ»±î µû·Î ÀÏ¹İ°ø°İ ÀÌº¥Æ®¸¦ ¸¸µé±î
    }

   IReadOnlyList<DiceEntry> BuildDice(ActionInstance action)
   {
        return action.Card.Dice;
   }
=======
=======
>>>>>>> d69ce474b7714212b5ec697ad90a7fa13dafd4dc
        if (graph.edges.TryGetValues(action.SourceSlot, out var otherSlot))
        {
            var otherAction = graph.GetAction(otherSlot);

            return new ClashInput(BuildDice(action), BuildDice(otherAction));
        }

        return new ClashInput(BuildDice(action), Array.Empty<DiceEntry>()); // ì—¬ê¸¸ ê³¼ì—° ë¹ˆê°’ìœ¼ë¡œ ì €ì¥í•´ë†”ë„ ë˜ëŠ” ê²ƒì¸ê°€
    }

    IReadOnlyList<DiceEntry> BuildDice(ActionInstance action)
    {
        return action.Card.Dice;
    }
<<<<<<< HEAD
>>>>>>> d69ce474b7714212b5ec697ad90a7fa13dafd4dc
=======
>>>>>>> d69ce474b7714212b5ec697ad90a7fa13dafd4dc
}
