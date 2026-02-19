using UnityEngine;

public class ActionPhaseSystem
{
    private BattleState state;
    public ActionPhaseSystem(BattleState state)
    {
        this.state = state;
    }

    public void RegisterAction(BattleState state, ActionInstance action)
    {
        state.RegisterAction(action);
    }


    public void CancelAction(BattleState state, SpeedSlot slot)
    {
        state.CancelAction(slot);
    }
}
