using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("적의 스텟")]
    [SerializeField] private int maxHP = 50;
    [SerializeField] private int maxEnergy = 3;

    public int currentHP { get; private set; }
    public int currentEnergy { get; private set; }

    [SerializeField] private BattleManager battlemanager;

    private void Awake()
    {
        currentHP = maxHP;
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
        if (currentHP <= 0)
        {
            Die();
        }
        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHP);
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
}
