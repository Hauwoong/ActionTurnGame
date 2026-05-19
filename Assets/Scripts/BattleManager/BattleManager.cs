using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private BattleRuntime _runtime;
    public BattleRuntime Runtime => _runtime;

    private BoutGraph _boutGraph;
    private int _nextRegisterOrder = 0;

    public event System.Action<BattleRuntime> OnBattleCreated;
    public event System.Action OnBattleEnded;

    public void CreateBattle(IEnumerable<Character> Characters)
    {
        int seed = new System.Random().Next();
        var snapShot = new BattleSnapShot(Characters, seed);
        _runtime = new BattleRuntime(snapShot);
        _boutGraph = new BoutGraph(
            new Dictionary<SpeedSlot, ActionInstance>(),
            _runtime.SlotRuntimeMap);

        OnBattleCreated?.Invoke(_runtime);
    }

    public void RegisterAction(SpeedSlot source, SpeedSlot target, CardData card)
    {
        var action = new ActionInstance(source, target, card, _nextRegisterOrder++);
        _boutGraph.RegisterAction(action);
    }

    public void StartTurn()
    {
        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnStartEvent(character.CharacterId));
    }

    public void EndTurn()
    {
        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnEndEvent(character.CharacterId));
    }

    public void EndBattle()
    {
        OnBattleEnded?.Invoke();
        _runtime = null;
    }
}
