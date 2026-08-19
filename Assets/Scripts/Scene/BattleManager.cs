using UnityEngine;
using System;
using System.Collections.Generic;

// BattleManager의 역할
// 1. BattleManager는 Scene(Unity)와 순수 엔진 사이의 다리이자 진입점 역할을 한다 -> Scene의 데이터가 순수 엔진이 만질 수 있도록 변환 과정을 거치도록 지시를 내림
// 2. BattleManager는 턴의 순서를 지배하고 소유하고 있다 -> 전투 생성/턴의 시작/전투 해석/턴 종료/전투 종료 하나의 전투 환경에서의 지휘자 역할을 함
// 3. Artwork의 레지스트리를 가지고 있다 -> Sprite는 UI 전용이기 때문에 엔진 CardModel(CardData(Asset 데이터) -> CardModel(순수 엔진 전용 데이터))에서 뺐고 대신 여기서 이름->Sprite를 들고 UI에 넘긴다.
public class BattleManager : MonoBehaviour
{
    private BattleRuntime _runtime;
    public BattleRuntime Runtime => _runtime;

    private readonly Dictionary<string, Sprite> _cardArtByName = new(); // Artwork는 UI 전용이고 CardModel은 Sprite가 없기 때문에 여기서 따로 이름으로 관리

    // UI가 여기 구독해서 LogDispatcher에 바인딩/해제하는 역할을 한다 / 런타임이 AddLog로 로그를 만들어냄 -> LogDispatcher가 로그 타입에 등록된 구독자(주로 UI)에게 전달한다.
    public event Action<BattleRuntime> OnBattleCreated;
    public event Action OnBattleEnded;

    /// <summary>
    /// 양 진영 씬 캐릭터를 받아 스냅샷->런타임을 만들고 OnBattleCreated를 쏜다.
    /// </summary>
    /// <param name="allies">아군 진영</param>
    /// <param name="enemies">적군 진영</param>
    public void CreateBattle(IEnumerable<Character> allies, IEnumerable<Character> enemies)
    {
        int seed = new System.Random().Next(); // 결정성은 seed 고정 시점부터 시작. 이 한 줄만 비결정적(실행마다 다름).
        var snapShot = new BattleSnapShot(ExtractData(allies), ExtractData(enemies), seed);
        _runtime = new BattleRuntime(snapShot);

        OnBattleCreated?.Invoke(_runtime); //OnBattleCreated라는 이벤트는 runtime 생성 이후 연결 -> runtime 생성이 되어야만 연결이 가능하다
    }

    /// <summary>
    /// Character(Scene Component) -> CharacterData(Asset)로 즉시 평가 + null 검증해서 List로 실체화 하는 역할을 한다.
    /// </summary>
    /// <param name="characters">데이터 추출할 캐릭터</param>
    /// <returns>등록된 캐릭터 데이터 List</returns>
    IEnumerable<CharacterModel> ExtractData(IEnumerable<Character> characters)
    {
        int index = 0;

        List<CharacterModel> datas = new List<CharacterModel>();
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

            foreach (var card in character.CharacterData.InitialDeck) // 겸사겸사 카드 artwork 레지스트리도 채운다
            {
                if (!_cardArtByName.ContainsKey(card.CardName))
                    _cardArtByName[card.CardName] = card.Artwork;
            }

            datas.Add(character.CharacterData.ToModel());
        }
        return datas;
    }

    /// <summary>
    /// 속도 주사위를 굴린 뒤 각 캐릭터에 TurnStartEvent를 Queueing한다.
    /// </summary>
    public void StartTurn()
    {
        if (_runtime == null) return;

        _runtime.RollSpeedDice();

        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnStartEvent(character.CharacterId)); // 주사위 굴림으로 속도 계산 -> 그후 턴시작 이벤트 활성화
    }

    /// <summary>
    /// 현재 BoutGraph 를 Executor 로 해석해 전투를 실행한다.
    /// </summary>
    public void ExecuteCombat()
    {
        if (_runtime == null) return;

        _runtime.Executor.Execute(_runtime.BoutGraph);
    }

    /// <summary>
    /// 한 턴을 마무리한다: 전투 해석 → TurnEndEvent → 판 비우기 → 종료 판정 → 다음 턴/전투 종료. 한 턴의 진행 순서를 여기서 소유한다.
    /// </summary>
    public void EndTurn()
    {
        if (_runtime == null) return;

        ExecuteCombat();

        foreach (var character in _runtime.Characters.Values)
            _runtime.EnqueueEvent(new TurnEndEvent(character.CharacterId)); // 전투 해석·턴 종료 훅을 먼저 돌리고 마지막에 판을 비운다. (현재 TurnEnd 는 BoutGraph 를 읽지 않아 순서 자체는 무해)

        _runtime.BoutGraph.Clear(); // 슬롯 표시 + 스테일 액션까지 전부 지움

        if (_runtime.TryGetBattleResult(out _))
            EndBattle();
        else
            StartTurn();
    }

    /// <summary>
    /// OnBattleEnded 를 쏘고 런타임을 버려 전투를 종료한다.
    /// </summary>
    public void EndBattle()
    {
        OnBattleEnded?.Invoke();
        _runtime = null;
    }

    /// <summary>
    /// 카드 이름으로 UI 용 artwork Sprite 를 조회한다(없으면 null).
    /// </summary>
    /// <param name="cardName">조회할 카드 이름</param>
    /// <returns>등록된 Sprite, 없으면 null</returns>
    public Sprite GetCardArtwork(string cardName)
    {
        if (_cardArtByName.TryGetValue(cardName, out var sprite))
            return sprite;
        else 
            return null;
    }

    // TODO: 3-1 검증용. 검증 끝나면 이 region 통째로 삭제.
    public void DebugAddBleed()
    {
        if (_runtime == null) return;

        _runtime.EnqueueEvent(new StatusAddEvent(0, StatusEffectType.Bleed, 5, false));
        _runtime.EnqueueEvent(new StatusAddEvent(1, StatusEffectType.Bleed, 5, false));
    }
    public void DebugAddStrength()
    {
        if (_runtime == null) return;

        _runtime.EnqueueEvent(new StatusAddEvent(0, StatusEffectType.Strength, 3, false));
    }
    public void DebugAddParalysis()
    {
        if (_runtime == null) return;

        _runtime.EnqueueEvent(new StatusAddEvent(0, StatusEffectType.Paralysis, 2, false));
    }
    public void DebugAddDelayedStrength()
    {
        if (_runtime == null) return;

        _runtime.EnqueueEvent(new StatusAddEvent(0, StatusEffectType.Strength, 3, true));
    }
}