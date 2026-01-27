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

    [Header("Starting Deck")]
    [SerializeField] private List<CardData> Deck;

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

        foreach (var card in Deck)
        {
            hand.Add(card);
        }
        OnHandChanged?.Invoke();
    }

    public void OnTurnStart()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy, MaxEnergy);
        Debug.Log("Player's Turn Started");
        Debug.Log("Player Energy Refreshed to " + currentEnergy);
        
        DrawCard();
        DrawCard();

        Debug.Log("Draw 2 cards");
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
        if(Deck.Count == 0)
        {
            Debug.Log("No cards left to draw.");
            return;
        }

        CardData card = Deck[0];
        Deck.RemoveAt(0);

        AddCard(card);
    }

    public void RemoveCard(CardData card)
    {
        hand.Remove(card);
        OnHandChanged?.Invoke();
    }

    public void AddCard(CardData card)
    {
        hand.Add(card);
        OnHandChanged?.Invoke();
    }

}
