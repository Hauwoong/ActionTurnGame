using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]

    [FormerlySerializedAs("cardName")]
    [SerializeField] private string _cardName;

    [FormerlySerializedAs("cost")]
    [SerializeField] private int _cost;

    [Header("Dice")]

    [FormerlySerializedAs("dices")]
    [SerializeField] private List<DiceData> _dices;

    [Header("Visual")]

    [FormerlySerializedAs("artwork")]
    [SerializeField] private Sprite _artwork;
    [TextArea]

    [FormerlySerializedAs("description")]
    [SerializeField] private string _description;

    public string CardName=> _cardName;
    public int Cost => _cost;
    public IReadOnlyList<DiceData> Dices => _dices;
    public Sprite Artwork => _artwork;
    public string Description => _description;

    public CardModel ToModel() => new CardModel(CardName, Cost, new List<DiceData>(Dices));
}