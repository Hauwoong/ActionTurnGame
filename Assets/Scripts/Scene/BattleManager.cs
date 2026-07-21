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
        var snapShot = new BattleSnapShot(ExtractData(allies), ExtractData(enemies), seed);
        _runtime = new BattleRuntime(snapShot);

        OnBattleCreated?.Invoke(_runtime);
    }

    IEnumerable<CharacterData> ExtractData(IEnumerable<Character> characters)
    {
        int index = 0;

        List<CharacterData> datas = new List<CharacterData>();
        foreach (var character in characters)
        {
            int current = index++;

            if (character == null)
            {
                Debug.LogError($"BattleManager: Character {current} is null.");
                continue;
            }

            if (character.CharacterData == null)
            {
                Debug.LogError($"BattleManager: CharacterData is null for character {character.name}.");
                continue;
            }

            datas.Add(character.CharacterData);
        }
        return datas;
    }

    public void StartTurn()
    {
        if (_runtime == null) return;

        _runtime.RollSpeedDice();

        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnStartEvent(character.CharacterId));
    }

    public void ExecuteCombat()
    {
        if (_runtime == null) return;

        _runtime.Executor.Execute(_runtime.BoutGraph);
    }

    // 턴 종료 -> 전투 해석 -> 종료 판정 -> 다음 턴. 한 턴의 진행 순서를 여기서 소유한다.
    public void EndTurn()
    {
        if (_runtime == null) return;

        ExecuteCombat();

        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnEndEvent(character.CharacterId));

        _runtime.BoutGraph.Clear();

        if (_runtime.TryGetBattleResult(out _))
            EndBattle();
        else
            StartTurn();
    }

    public void EndBattle()
    {
        OnBattleEnded?.Invoke();
        _runtime = null;
    }
}