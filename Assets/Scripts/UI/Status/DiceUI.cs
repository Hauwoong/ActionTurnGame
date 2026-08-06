using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI diceRollText;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int maxLines = 15;

    private BattleRuntime _runtime;
    private readonly Queue<string> _lines = new();

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

        _runtime.LogDispatcher.Register<DiceClashLog>(OnDiceClash);
        _runtime.LogDispatcher.Register<UnopposedLog>(OnUnopposed);
        _runtime.LogDispatcher.Register<DamageLog>(OnDamage);
        _runtime.LogDispatcher.Register<DiceConsumedLog>(OnDiceConsumed);
        _runtime.LogDispatcher.Register<DiceDestroyedLog>(OnDiceDestroyed);
        _runtime.LogDispatcher.Register<DiceReusedLog>(OnDiceReused);
        _runtime.LogDispatcher.Register<BoutStartLog>(OnBoutStart);
        _runtime.LogDispatcher.Register<StaggerLog>(OnStagger);

        Clear();
    }
    private void Unbind()
    {
        if (_runtime == null) return;

        _runtime.LogDispatcher.Unregister<DiceClashLog>(OnDiceClash);
        _runtime.LogDispatcher.Unregister<UnopposedLog>(OnUnopposed);
        _runtime.LogDispatcher.Unregister<DamageLog>(OnDamage);
        _runtime.LogDispatcher.Unregister<DiceConsumedLog>(OnDiceConsumed);
        _runtime.LogDispatcher.Unregister<DiceDestroyedLog>(OnDiceDestroyed);
        _runtime.LogDispatcher.Unregister<DiceReusedLog>(OnDiceReused);
        _runtime.LogDispatcher.Unregister<BoutStartLog>(OnBoutStart);
        _runtime.LogDispatcher.Unregister<StaggerLog>(OnStagger);

        _runtime = null;
    }
    private void Append(string line)
    {
        _lines.Enqueue(line);
        while (_lines.Count > maxLines)
        {
            _lines.Dequeue();
        }
        Redraw();
    }
    private void Redraw()
    {
        if (diceRollText == null) return;

        diceRollText.text = _lines.Count > 0 ? string.Join(("\n"), _lines) : "-";
    }
    private void OnDiceClash(DiceClashLog log)
    {
        Append("[Clash] "+Describe(log.HandleA) + " " + Roll(log.BaseRollA, log.ModifiedRollA)
            + " vs "
            + Describe(log.HandleB) + " " + Roll(log.BaseRollB, log.ModifiedRollB)
            + " => "
            + log.Result.ToString() + " | A:"
            + log.AdvanceA.ToString() + " B:" + log.AdvanceB.ToString());
    }
    private void OnUnopposed(UnopposedLog log)
    {
        Append("[Unopp] "+Describe(log.Handle) + " " + Roll(log.BaseRoll, log.ModifiedRoll) + " To " + log.TargetId.ToString());
          
    }
    private void OnDamage(DamageLog log)
    {
        Append("[Dmg] "+ log.AttackerId.ToString() + " -> " + log.TargetId.ToString() + " takes " + log.Amount.ToString());
    }
    private void OnDiceConsumed(DiceConsumedLog log)
    {
        Append("[Stored] "+Describe(log.Handle));
    }
    private void OnDiceDestroyed(DiceDestroyedLog log)
    {
        Append("[Destroyed] "+Describe(log.Handle));
    }
    private void OnDiceReused(DiceReusedLog log)
    {
        Append("[Reused] "+Describe(log.Handle));
    }
    private void OnBoutStart(BoutStartLog log)
    {
        Append("[Bout] "+log.AttackerId.ToString() + " -> " + log.TargetId.ToString() + (log.WasClash ? " (clash)" : " (unopposed)"));
    }
    private void OnStagger(StaggerLog log)
    {
        Append("[Stagger] "+log.AttackerId.ToString() + " -> " + log.CharacterId.ToString() + " takes " + log.Amount + (log.IsRecover ? " (recover)" : " (staggered)"));
    }
    private void Clear()
    {
        _lines.Clear();
        Redraw();
    }
    private string Describe(DiceHandle handle)
    {
        var owner = _runtime.GetCharacterRuntime(handle.Owner.CharacterId);
        var dice = owner.GetDiceInfo(handle.DiceId);

        if (dice.HasValue)
        {
            return $"[id:{handle.Owner.CharacterId} {dice.Value.Type} {dice.Value.Min}~{dice.Value.Max}]";
        }
        return $"[id:{handle.Owner.CharacterId} ?]";
    }
    private string Roll(int baseRoll, int modified)
    {
        if (baseRoll == modified)
        {
            return $"[{baseRoll}]";
        }
        return $"[{baseRoll}->{modified}]";
    }
}
