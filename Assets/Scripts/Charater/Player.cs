using System;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int maxEnergy = 3;

    // Public getters for max stats
    public int MaxHP => maxHP;
    public int MaxEnergy => maxEnergy;

    public int currentHP { get; private set; }
    public int currentEnergy { get; private set; }

    [Header("References")]
    [SerializeField] private BattleManager battlemanager;
    [SerializeField] private TurnManager turnManager;

    // UI 업데이트를 위한 이벤트
    public event Action<int, int> OnHPChanged;
    public event Action<int, int> OnEnergyChanged;
    public event Action OnHandChanged;

    public List<CardData> hand = new List<CardData>();

    private void Awake()
    {
        currentHP = maxHP;
        currentEnergy = maxEnergy;
    }

    private void Start()
    {
        if (battlemanager == null || turnManager == null)
        {
            Debug.LogError("Player: Missing Reference to BattleManager or TurnManager.");
        }
    }

    public void OnTurnStart()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy, MaxEnergy);
        Debug.Log("Player's Turn Started");
        Debug.Log("Player Energy Refreshed to " + currentEnergy);
        
        DrawCard();
        DrawCard();
        DrawCard();

        Debug.Log("Draw 3 cards");
    }

    public void UseCard(int index)
    {
        if (index < 0 || index >= hand.Count)
        {
            Debug.Log("Invalid card index.");
            return;
        }

        CardData card = hand[index];

        if (!UseEnergy(card.cost))
        {
            return;
        }

        card.Use(this, battlemanager.CurrentEnemy);

        hand.RemoveAt(index);
        OnHandChanged?.Invoke();
    }
    
    public bool UseEnergy(int cost)
    {
        if (cost <= currentEnergy)
        {
            currentEnergy -= cost;
            Debug.Log("Card used with cost " + cost + ". Remaining energy: " + currentEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, MaxEnergy);
            return true;
        }
        else
        {
            Debug.Log("Not enough energy to use the card.");
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(currentHP - damage, 0);

        OnHPChanged?.Invoke(currentHP, MaxHP);

        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        battlemanager.EndBattle(false);
       
    }

    public void RequestTurnEnd()
    {
        turnManager.PlayerEndTurn();
    }

    void DrawCard()
    {
        CardData card = new CardData(); // 카드 생성 로직 필요
        card.cardName = "Attack";
        card.cost = 1;
        card.damage = 10;

        hand.Add(card);
        OnHandChanged?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseCard(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseCard(1);
        }
    }
}
