using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Game/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private string _name;

    [Header("Stats")]
    [SerializeField] private int _maxHp;
    [SerializeField] private int _maxStagger;
    [SerializeField] private int _maxEnergy;
    [SerializeField] private int _minSpeed;
    [SerializeField] private int _maxSpeed;
    [SerializeField] private int _baseSpeedSlotCount = 1;

    [Header("Emotion")]
    [SerializeField] private int _maxEmotionLevel = 5;     // 최대 감정 단계
    [SerializeField] private int _maxEmotionStack = 10;    // 감정 스택 최대치 감정 스택 10 -> 감정 레벨 +1
    [SerializeField] private int _emotionGainOnDamageDealt;
    [SerializeField] private int _emotionGainOnDamageReceived;
    [SerializeField] private int _emotionGainOnStagger;
    [SerializeField] private int _emotionGainOnStaggered;
    [SerializeField] private int _emotionGainOnStaggerHeal;

    [Header("Passives & Deck")]
    [SerializeField] private List<PassiveData> _passives = new();
    [SerializeField] private List<CardData> _initialDeck = new();
    public IReadOnlyList<CardData> InitialDeck => _initialDeck;

    public CharacterModel ToModel()
    {
        var passives = new List<PassiveModel>();
        foreach (var passive in _passives)
        {
            passives.Add(passive.ToModel());
        }

        var intialDeck = new List<CardModel>();
        foreach (var card in _initialDeck)
        {
            intialDeck.Add(card.ToModel());
        }

        return new CharacterModel(
            name : _name,
            maxHp : _maxHp,
            maxStagger : _maxStagger,
            maxEnergy : _maxEnergy,
            minSpeed : _minSpeed,
            maxSpeed : _maxSpeed,
            baseSpeedSlotCount : _baseSpeedSlotCount,
            maxEmotionLevel : _maxEmotionLevel,
            maxEmotionStack : _maxEmotionStack,
            emotionGainOnDamageDealt : _emotionGainOnDamageDealt,
            emotionGainOnDamageReceived : _emotionGainOnDamageReceived,
            emotionGainOnStagger : _emotionGainOnStagger,
            emotionGainOnStaggered : _emotionGainOnStaggered,
            emotionGainOnStaggerHeal : _emotionGainOnStaggerHeal,
            passives : passives,
            initialDeck : intialDeck
        );
    }

}
