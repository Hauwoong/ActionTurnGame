using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    [Header("Speed Dice")]
    public int diceCount = 1;

    public List<SpeedDice> speedDices = new();

    public List<int> rolledSpeeds = new();

    protected virtual void Awake()
    {
        InitSpeedDice();
    }

    void InitSpeedDice()
    {
        speedDices.Clear();

        for (int i = 0; i < diceCount; i++)
        {
            speedDices.Add(new SpeedDice());
        }
    }

    public void RollspeedDice()
    {
        rolledSpeeds.Clear();

        foreach (var dice in speedDices)
        {
            dice.Roll();
            rolledSpeeds.Add(dice.value);
        }
    }
}
