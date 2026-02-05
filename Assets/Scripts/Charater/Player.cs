using System;
using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    [Header("Starting Deck")]
    [SerializeField] private List<CardData> Deck;
    public List<CardData> hand = new List<CardData>();

    [Header("References")]
    [SerializeField] private BattleManager battlemanager;

    // UI 업데이트를 위한 이벤트
    public event Action<int, int> OnHPChanged;
    public event Action<int, int> OnEnergyChanged;
    public event Action OnHandChanged;
    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Player Ready");

    }

    private void Start()
    {
        if (battlemanager == null)
        {
            Debug.LogError("Player: Missing Reference to BattleManager or TurnManager.");
        }

        foreach (var card in Deck)
        {
            hand.Add(card);
        }
        OnHandChanged?.Invoke();
    }
 
    // 자원 관련 메소드

    public override void UseEnergy(int amount)
    {
        base.UseEnergy(amount);
        OnEnergyChanged?.Invoke(currentEnergy, MaxEnergy);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        OnHPChanged?.Invoke(currentHP, MaxHP);

        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);

        OnHPChanged?.Invoke(currentHP, MaxHP);

        Debug.Log("Player healed " + amount + " HP. Current HP: " + currentHP);
    }
    public override void Die()
    {
        base.Die();
        battlemanager.EndBattle(false);
       
    }

    // 카드 관련 메소드

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

    // 턴 관련 메소드
    public override void OnTurnStart()
    {
        base.OnTurnStart();

        OnEnergyChanged?.Invoke(currentEnergy, MaxEnergy);
       
        DrawCard();

        Debug.Log("Draw 1 cards");
    }
}
