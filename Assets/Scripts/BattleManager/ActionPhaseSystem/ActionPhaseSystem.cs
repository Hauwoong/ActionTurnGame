using UnityEngine;

public class ActionPhaseSystem
{
    public void RegisterAction(BattleState state, ActionInstance action)
    {
        state.RegisterActions.Add(action);
        state.BoutGraph.RegisterAction(action);
    }
       
}
