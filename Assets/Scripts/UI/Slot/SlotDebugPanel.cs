using System.Collections.Generic;
using UnityEngine;

public class SlotDebugPanel : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private SlotDebugItem itemPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private PlayerActionInput input;

    private BattleRuntime _runtime;
    private readonly List<SlotDebugItem> _items = new();
    private readonly List<SpeedSlotRuntime> _sorted = new();

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
        runtime.LogDispatcher.Register<TurnStartLog>(OnTurnStart);
        runtime.LogDispatcher.Register<ActionRegisteredLog>(OnActionRegistered);
        runtime.LogDispatcher.Register<ActionCancelledLog>(OnActionCancelled);
        runtime.LogDispatcher.Register<BoutStartLog>(OnBoutStart);
        runtime.LogDispatcher.Register<BoutEndLog>(OnBoutEnd);
        Refresh();
    }

    private void Unbind()
    {
        if (_runtime == null) return;
        _runtime.LogDispatcher.Unregister<TurnStartLog>(OnTurnStart);
        _runtime.LogDispatcher.Unregister<ActionRegisteredLog>(OnActionRegistered);
        _runtime.LogDispatcher.Unregister<ActionCancelledLog>(OnActionCancelled);
        _runtime.LogDispatcher.Unregister<BoutStartLog>(OnBoutStart);
        _runtime.LogDispatcher.Unregister<BoutEndLog>(OnBoutEnd);
        _runtime = null;
    }

    private void OnTurnStart(TurnStartLog log) => Refresh();
    private void OnActionRegistered(ActionRegisteredLog log) => Refresh();
    private void OnActionCancelled(ActionCancelledLog log) => Refresh();
    private void OnBoutStart(BoutStartLog log)
    {
        Refresh();
    }
    private void OnBoutEnd(BoutEndLog log) => Refresh();

    private static int CompareBySpeed(SpeedSlotRuntime a, SpeedSlotRuntime b)
    {
        int speedCompare = b.Speed.CompareTo(a.Speed);
        if (speedCompare != 0) return speedCompare;

        int characterCompare = a.Slot.CharacterId.CompareTo(b.Slot.CharacterId);
        if (characterCompare != 0) return characterCompare;

        return a.Slot.SlotIndex.CompareTo(b.Slot.SlotIndex);
    }

    private void Refresh()
    {
        Clear();

        if (_runtime == null) return;

        foreach (var character in _runtime.Characters.Values)
            _sorted.AddRange(character.SpeedSlotPool);

        _sorted.Sort(CompareBySpeed);

        foreach (var slot in _sorted)
        {
            var item = Instantiate(itemPrefab, container);
            item.Bind(slot, _runtime.BoutGraph, input);
            _items.Add(item);
        }
    }

    private void Clear()
    {
        foreach (var item in _items)
            Destroy(item.gameObject);
        _items.Clear();
        _sorted.Clear();
    }
}
