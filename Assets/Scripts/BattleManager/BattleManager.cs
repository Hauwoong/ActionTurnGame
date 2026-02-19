
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour
{
    private BattleState state = new();
    public BattleState State => state;

    private ResolutionSystem resolution;
    public ResolutionSystem Resolution => resolution;

    private ActionPhaseSystem actionPhase;
    public ActionPhaseSystem ActionPhase => actionPhase;    

    public event Action<BattleState> OnBattleStateChanged;

    public void Awake()
    {
        actionPhase = new ActionPhaseSystem(state);
        resolution = new ResolutionSystem(state);
    }

    public void StartTurn()
    {
        state.StartNewTurn();
        OnBattleStateChanged?.Invoke(state);
    }

    public void EndTurn()
    {
        resolution.Resolve(state);
        OnBattleStateChanged?.Invoke(state);

        StartTurn();
    }

    public void RegisterAction(SpeedSlot sourceSlot, SpeedSlot targetSlot, CardData card)
    {
        var action = new ActionInstance
        {
            SourceSlot = sourceSlot,
            TargetSlot = targetSlot,
            Card = card
        };
        actionPhase.RegisterAction(state, action);
        OnBattleStateChanged?.Invoke(state);
    }

    public void CancelAction(SpeedSlot slot)
    {
        actionPhase.CancelAction(state, slot);
        OnBattleStateChanged?.Invoke(state);
    }
}
