using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    public Player player;
    public Enemy enemy;
    public TurnState State { get; private set; }

    public event Action<TurnState> OnTurnChanged;

    public enum TurnState 
    {
        PlayerTurn,
        EnemyTurn
    }

    void Start()
    {
        State = TurnState.PlayerTurn;
        OnTurnChanged?.Invoke(State);
        BeginTurn();
    }

    void BeginTurn()
    {
        OnTurnChanged?.Invoke(State);

        if (State == TurnState.PlayerTurn)
        {
            if (player == null)
            {
                Debug.LogError("Player reference is missing in TurnManager.");
                return;
            }
            player.OnTurnStart();
        }
        else if (State == TurnState.EnemyTurn)
        {
            if (enemy == null)
            {
                Debug.LogError("Enemy reference is missing in TurnManager.");
                return;
            }
            enemy.OnTurnStart();
        }
    }
    
    public void EndTurn()
    {
        player.ClearDiceStack();
        enemy.ClearDiceStack();
    }
}
