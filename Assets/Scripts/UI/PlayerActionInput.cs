using UnityEngine;

public class PlayerActionInput : MonoBehaviour
{
    public Player player;
    public BattleManager battleMananger;

    private int selectedDiceIndex = -1;

    public void SelectSpeedDice(int index)
    {
        selectedDiceIndex = index;

        Debug.Log($"Speed Dice {index} selected");
    }

    public void SelectCard(CardData card, Character target)
    {
        if (selectedDiceIndex == -1)
        {
            Debug.Log("Select speed dice first!");
            return;
        }

        battleMananger.RegisterAction(player, target, card, selectedDiceIndex);

        selectedDiceIndex = -1; // 선택된 속도 주사위 인덱스 값 초기화
    }
}
