using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class Enemy : Character
{
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

    public override void OnTurnStart()
    {
        currentEnergy = maxEnergy;

        Debug.Log("Enemy's Turn Started");
        Debug.Log("Enemy Energy Refreshed to " + currentEnergy);
        // 적의 행동 로직
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();

        battlemanager.EndBattle(true);
       
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
