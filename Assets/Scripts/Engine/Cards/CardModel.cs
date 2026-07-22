using System.Collections.Generic;

public class CardModel
{
    public string CardName { get; }
    public int Cost { get; }
    public IReadOnlyList<DiceData> Dices  { get; }


    public CardModel(string cardName, int cost, List<DiceData> dices)
    {
        CardName = cardName;
        Cost = cost;
        Dices = new List<DiceData>(dices);
    }
}
