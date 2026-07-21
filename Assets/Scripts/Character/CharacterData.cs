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

    public string Name => _name;
    public int MaxHp => _maxHp;
    public int MaxStagger => _maxStagger;
    public int MaxEnergy => _maxEnergy;
    public int MinSpeed => _minSpeed;
    public int MaxSpeed => _maxSpeed;
    public int BaseSpeedSlotCount => _baseSpeedSlotCount;
    public int MaxEmotionLevel => _maxEmotionLevel;
    public int MaxEmotionStack => _maxEmotionStack;
    public int EmotionGainOnDamageDealt => _emotionGainOnDamageDealt;
    public int EmotionGainOnDamageReceived => _emotionGainOnDamageReceived;
    public int EmotionGainOnStagger => _emotionGainOnStagger;
    public int EmotionGainOnStaggered => _emotionGainOnStaggered;
    public int EmotionGainOnStaggerHeal => _emotionGainOnStaggerHeal;
    public IReadOnlyList<PassiveData> Passives => _passives;
    public IReadOnlyList<CardData> InitialDeck => _initialDeck;
}
