
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private BattleRuntime _runtime;
    public BattleRuntime Runtime => _runtime;

    public void CreateBattle(IEnumerable<Character> characters)
    {
        int seed = new System.Random().Next();

        var snapshot = new BattleSnapShot(characters, seed);

        _runtime = new BattleRuntime(snapshot);

        _runtime.RollSpeedDice();
    }

    public void SubmitInput(BattleInput input)
    {
        if (_runtime == null) return;

        _runtime.EnqueueEvent(new ClashResolveEvent(input.BoutGraph));
    }

    public bool Step()
    {
        if (_runtime == null) return false;

        _runtime.Step();
        return true;
    }
}
