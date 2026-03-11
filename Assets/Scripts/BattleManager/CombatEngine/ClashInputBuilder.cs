public class ClashInputBuilder
{
    public ClashInput Build(BoutGraph graph, ActionInstance action)
    {
        if (graph.edges.TryGetValues(action.SourceSlot, out var otherSlot))
        {
            var otherAction = graph.GetAction(otherSlot);

            return new ClashInput(BuildDice(action), BuildDice(otherAction));
        }

        return new ClashInput(BuildDice(action), Array.Empty<DiceEntry>()); // 여길 과연 빈값으로 저장해놔도 되는 것인가
    }

    IReadOnlyList<DiceEntry> BuildDice(ActionInstance action)
    {
        return action.Card.Dice;
    }
}
