using System.Collections.Generic;
using UnityEngine;

public class CardHandUI : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int characterId;
    [SerializeField] private PlayerActionInput input;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;

    private BattleRuntime _runtime;
    private readonly List<GameObject> _cardObjects = new();

    private void OnEnable()
    {
        battleManager.OnBattleCreated += Bind;
        battleManager.OnBattleEnded += Unbind;
        if (battleManager.Runtime != null)
            Bind(battleManager.Runtime);
    }

    private void OnDisable()
    {
        battleManager.OnBattleCreated -= Bind;
        battleManager.OnBattleEnded -= Unbind;
        Unbind();
    }

    private void Bind(BattleRuntime runtime)
    {
        Unbind();
        _runtime = runtime;
        runtime.LogDispatcher.Register<DrawCardLog>(OnDrawCard);
        runtime.LogDispatcher.Register<UseCardLog>(OnUseCard);
        runtime.LogDispatcher.Register<DiscardCardLog>(OnDiscardCard);
        runtime.LogDispatcher.Register<ExileCardLog>(OnExileCard);
        runtime.LogDispatcher.Register<EndTurnCardLog>(OnEndTurnCard);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<DrawCardLog>(OnDrawCard);
        _runtime.LogDispatcher.Unregister<UseCardLog>(OnUseCard);
        _runtime.LogDispatcher.Unregister<DiscardCardLog>(OnDiscardCard);
        _runtime.LogDispatcher.Unregister<ExileCardLog>(OnExileCard);
        _runtime.LogDispatcher.Unregister<EndTurnCardLog>(OnEndTurnCard);
        _runtime = null;
    }

    private void OnDrawCard(DrawCardLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnUseCard(UseCardLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnDiscardCard(DiscardCardLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnExileCard(ExileCardLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void OnEndTurnCard(EndTurnCardLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }

    private void Refresh()
    {
        if (_runtime == null) return;

        foreach (var obj in _cardObjects)
            Destroy(obj);
        _cardObjects.Clear();

        var cr = _runtime.GetCharacterRuntime(characterId);
        foreach (var card in cr.CardManager.Hand)
        {
            var obj = Instantiate(cardPrefab, cardParent);
            obj.GetComponent<CardUI>().Setup(card, input);
            _cardObjects.Add(obj);
        }
    }
}
