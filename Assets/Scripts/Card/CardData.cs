using UnityEngine;

[CreateAssetMenu(fileName = "New card", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public int cost;
    public int damage;

    [Header("Visual")]
    public Sprite artwork;

    [TextArea(3, 5)]
    public string description;

    public virtual bool Use(Player player, Enemy enemy)
    {
        if (!player.UseEnergy(cost))
        {
            return false;
        }
        enemy.TakeDamage(damage);
        return true;
    }
}