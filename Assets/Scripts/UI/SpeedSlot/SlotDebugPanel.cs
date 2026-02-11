using UnityEngine;
using System.Collections.Generic;

public class SlotDebugPanel : MonoBehaviour
{
    public BattleManager battle;

    public Transform playerGroup;
    public Transform enemyGroup;

    public SlotDebugItem itemPrefab;
    public PlayerActionInput input;

    List<SlotDebugItem> items = new();

    public void Refresh()
    {
        Clear();

        foreach (Character unit in battle.Units)
        {
            foreach (SpeedSlot slot in unit.speedSlots)
            {
                var parent =
                    unit is Player ? playerGroup : enemyGroup;

                var item = Instantiate(itemPrefab, parent);

                item.Init(slot, input);
                items.Add(item);
            }
        }
    }

    void Clear()
    {
        foreach (var i in items)
        {
            Destroy(i.gameObject);
        }
        items.Clear();
    }
}
