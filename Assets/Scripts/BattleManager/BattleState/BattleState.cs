using System.Collections.Generic;

public class BattleState
{
    public List<Character> Units = new();
    public List<ActionInstance> RegisterActions = new();
    public BoutGraph BoutGraph = new();


    public void StartNewTurn()
    {
        RegisterActions.Clear();

        foreach (var unit in Units)
        {
            unit.RollspeedDice();
        }
    }
}
