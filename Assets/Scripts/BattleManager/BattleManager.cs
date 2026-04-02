
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    private BattleRuntime _runtime;
    public BattleRuntime Runtime => _runtime;

    public void CreateBattle(IEnumerable<Character> Characters)
    {
        int seed = new System.Random().Next();
        var snapShot = new BattleSnapShot(Characters, seed);
        _runtime = new BattleRuntime(snapShot);

    }
    public void StartTurn()
    {
        // 모든 캐릭터 턴 시작 트리거
        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnStartEvent(character.CharacterId));
    }
    public void EndTurn()
    {
        // 모든 캐릭터 턴 종료 트리거
        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnEndEvent(character.CharacterId));
    }
}
