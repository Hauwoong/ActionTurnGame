using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Enemy : Character
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 50;
    [SerializeField] private int maxEnergy = 3;

    public int currentHP { get; private set; }
    public int currentEnergy { get; private set; }

    [Header("Deck")]
    [SerializeField] private List<CardData> deck = new();
    public List<CardData> hand = new();

    [SerializeField] private BattleManager battlemanager;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Enemy Ready");

        currentHP = maxHP;

        foreach (var card in deck)
        {
            hand.Add(card);
        }
    }

    public void OnTurnStart()
    {
        currentEnergy = maxEnergy;

        Debug.Log("Enemy's Turn Started");
        Debug.Log("Enemy Energy Refreshed to " + currentEnergy);
        // 적의 행동 로직
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died.");

        if (battlemanager != null)
        {
            battlemanager.EndBattle(true);
        }
        else
        {
            Debug.Log("battlemanager not assigned");
        }
    }

    public CardData GetRandomCard()
    {
        if (deck.Count == 0)
        {
            return null;
        }
        int r = Random.Range(0,hand.Count);
        return hand[r];
    }
}
