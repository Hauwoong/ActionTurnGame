using UnityEngine;

public class PlayerActionInput : MonoBehaviour
{
    public Player player;
    public BattleManager battleManager;

    SpeedSlot speedSlot = null;

    public void SelectSpeedSlot(SpeedSlot slot)
    {
        speedSlot = slot;

        Debug.Log($"Speed Dice {speedSlot.index} selected");
    }

    public void SelectCard(SpeedSlot targetSlot,CardData card)
    {
        if (speedSlot == null)
        {
            Debug.Log("Select speed Slot first!");
            return;
        }

        if (speedSlot.IsUsed)
        {
            Debug.Log("This slot already used!");
            return;
        }

        battleManager.RegisterAction(speedSlot,targetSlot,card);

        speedSlot = null; // 선택된 속도 주사위 인덱스 값 초기화
    }
}
