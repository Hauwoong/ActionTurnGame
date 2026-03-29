using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] protected string charsetName;
    public string Name => charsetName;

    [Header("Stats")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected int maxEnergy;
    [SerializeField] protected int maxStagger;
    public int MaxHP => maxHP;
    public int MaxEnergy => maxEnergy;
    public int MaxStagger => maxStagger;

    public int currentHP { get; protected set; }
    public int currentEnergy { get ; protected set; }

    public int MinSpeed { get; protected set; }
    public int MaxSpeed { get; protected set; }

    [Header("Speed Dice")]
    public int diceCount = 1;
    public List<SpeedSlot> speedSlots = new();
    public List<int> rolledSpeeds = new();

    protected virtual void Awake()
    {
        currentHP = maxHP;
        currentEnergy = maxEnergy;
    }
}
