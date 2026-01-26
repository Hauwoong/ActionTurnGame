using UnityEngine;

[System.Serializable]
public class CardData
{
    public string cardName;
    public int cost;
    public int damage;

    public void Use(Player player, Enemy enemy)
    {
        if (player.UseEnergy(cost))
        {
            enemy.TakeDamage(damage);
            Debug.Log(cardName+"»ç¿ë");
        }
    }
}