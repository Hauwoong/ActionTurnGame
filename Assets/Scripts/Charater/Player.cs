using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int maxEnergy = 3;

    public int MaxHP => maxHP;
    public int MaxEnergy => maxEnergy;

    public int currentHP { get; private set; }
    public int currentEnergy { get; private set; }

    [Header("References")]
    [SerializeField] private BattleManager battlemanager;

    public event Action<int> OnHPChanged;

    private void Awake()
    {
        currentHP = maxHP;
        currentEnergy = maxEnergy;
        OnHPChanged?.Invoke(currentHP);
    }

    public void OnTurnStart()
    {
        currentEnergy = maxEnergy;

        Debug.Log("Player's Turn Started");
        Debug.Log("Player Energy Refreshed to " + currentEnergy);
        // 카드 드로우
        // UI 활성화
    }

    public bool UseCard(int cost) 
    {
        if (cost <= currentEnergy)
        {
            currentEnergy -= cost;
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

        OnHPChanged?.Invoke(currentHP);

        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");

        if (battlemanager != null)
        {
            battlemanager.EndBattle(false);
        }
        else
        {
            Debug.Log("battlemanager not assigned");
        }
    }


}
