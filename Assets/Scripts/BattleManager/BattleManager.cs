using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private BattleRuntime _runtime;
    public BattleRuntime Runtime => _runtime;

    public event Action<BattleRuntime> OnBattleCreated;
    public event Action OnBattleEnded;

    public void CreateBattle(IEnumerable<Character> allies, IEnumerable<Character> enemies)
    {
        int seed = new System.Random().Next();
        var snapShot = new BattleSnapShot(allies, enemies, seed);
        _runtime = new BattleRuntime(snapShot);

        OnBattleCreated?.Invoke(_runtime);
    }

    public void StartTurn()
    {
        _runtime.RollSpeedDice();

        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnStartEvent(character.CharacterId));
    }

    public void ExecuteCombat()
    {
        _runtime.Executor.Execute(_runtime.BoutGraph);
    }

    public void EndTurn()
    {
        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnEndEvent(character.CharacterId));

        _runtime.BoutGraph.Clear();
    }

    public void EndBattle()
    {
        OnBattleEnded?.Invoke();
        _runtime = null;
    }
}