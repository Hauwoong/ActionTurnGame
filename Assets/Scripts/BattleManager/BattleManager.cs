
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour
{
    private BattleState state;
    private ResolutionSystem resolution;

    public event Action<BattleState> OnBattleStateChanged;

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
}
