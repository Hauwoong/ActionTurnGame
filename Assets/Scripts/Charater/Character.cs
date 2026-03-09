using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;

public class Character : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] protected string charsetName;
    public string Name => charsetName;

    [Header("Stats")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected int maxEnergy;
    public int MaxHP => maxHP;
    public int MaxEnergy => maxEnergy;

    public int currentHP { get; protected set; }
    public int currentEnergy { get ; protected set; }

    public int MinSpeed { get; protected set; }
    public int MaxSpeed { get; protected set; }

    [Header("Speed Dice")]
    public int diceCount = 1;
    public List<SpeedSlot> speedSlots = new();
    public List<int> rolledSpeeds = new();

    [Header("주사위 스택")]
    protected Queue<DiceRuntime> diceStack = new();
    public bool HasDice => diceStack.Count > 0;
    //public bool[] usedSlot;
    protected virtual void Awake()
    {
        currentHP = maxHP;
        currentEnergy = maxEnergy;
    }

    // 주사위 관련 메소드

    public void RollspeedDice()
    {
        rolledSpeeds.Clear();
        speedSlots.Clear();

        for (int i = 0; i < diceCount; i++)
       {
            int v = Random.Range(1, 9);
            rolledSpeeds.Add(v);

            speedSlots.Add(new SpeedSlot
            {
                owner = this,
                index = i
            });
       }
    }

    public void PushDice(DiceRuntime dice)
    {
        diceStack.Enqueue(dice);
    }

    public DiceRuntime PopDice()
    {
        if (diceStack.Count == 0) return null;
        return diceStack.Dequeue();
    }

    public void ClearDiceStack()
    {
        diceStack.Clear();
    }

    public virtual void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(currentHP - dmg, 0);

        Debug.Log($"{Name} took {dmg} damage. HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public virtual void UseEnergy(int amount)
    {
        currentEnergy -= amount;
        Debug.Log($"{Name} used {amount} Energy. Current Energy: {currentEnergy}");
    }

    // 행동 메소드
    public virtual void OnTurnStart()
    {
        //usedSlot = new bool[diceCount];
        currentEnergy = maxEnergy;
        Debug.Log($"{Name} Energy Refreshed to {currentEnergy}");
    }

    public virtual void Die()
    {
        Debug.Log($"{Name} died");
    }
}
