using UnityEngine;

public class ActionPhaseSystem
{
    public void RegisterAction(BattleState state, ActionInstance action)
    {
        state.RegisterAction(action);
    }


    public void CancelAction(BattleState state, SpeedSlot slot)
    {
        state.CancelAction(slot);
    }
}
