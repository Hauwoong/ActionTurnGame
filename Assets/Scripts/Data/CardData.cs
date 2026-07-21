using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public int cost;

    [Header("Dice")]
    public List<DiceData> dices;

    [Header("Visual")]
    public Sprite artwork;
    [TextArea]
    public string description;
}