using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int characterId;

    private BattleRuntime _runtime;

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

        _runtime.LogDispatcher.Register<StatusAddLog>(OnStatusAdd);
        _runtime.LogDispatcher.Register<StatusDamageLog>(OnStatusDamage);
        _runtime.LogDispatcher.Register<TurnEndLog>(OnTurnEnd);
        _runtime.LogDispatcher.Register<TurnStartLog>(OnTurnStart);

        Refresh();
    }
    private void Unbind()
    {
        if (_runtime == null) return;

        _runtime.LogDispatcher.Unregister<StatusAddLog>(OnStatusAdd);
        _runtime.LogDispatcher.Unregister<StatusDamageLog>(OnStatusDamage);
        _runtime.LogDispatcher.Unregister<TurnEndLog>(OnTurnEnd);
        _runtime.LogDispatcher.Unregister<TurnStartLog>(OnTurnStart);

        _runtime = null;
    }

    private void OnStatusAdd(StatusAddLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }
    private void OnStatusDamage(StatusDamageLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }
    private void OnTurnEnd(TurnEndLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }
    private void OnTurnStart(TurnStartLog log)
    {
        if (log.CharacterId == characterId) Refresh();
    }
    private void Refresh()
    {
        if (_runtime == null) return;
        var cr = _runtime.GetCharacterRuntime(characterId);

        var parts = new List<string>();

        foreach (var effect in cr.StatusEffects)
        {
            if (effect.IsExpired) continue;

            string part = effect.Type.ToString() + " " + effect.Stack.ToString();

            if (effect.PendingStack > 0) part += " (+" + effect.PendingStack + ")";

            parts.Add(part);
        }

        if (statusText != null)
        {
           statusText.text = parts.Count > 0 ? string.Join(" | ", parts) : "-";  
        }
    }
}