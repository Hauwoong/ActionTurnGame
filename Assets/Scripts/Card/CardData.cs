using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Game/Card")]
public abstract class CardData : ScriptableObject
{
    
    [Header("Card Info")]
    public string cardName;
    public int cost;

    [Header("Dice")]
    public List<DiceData> dices;

    [Header("Visual")]
    public Sprite artwork;

    [TextArea]
    public string description;

    public bool CanUse(Player player)
    {
        return player.currentEnergy >= cost;
    }

    public virtual void Use(BattleContext ctx)
    {
        ResolveCard(ctx);
    }

    protected virtual void ResolveCard(BattleContext ctx)
    {
        foreach (var dice in dices)
        {
            ctx.currentDice = dice;
            ctx.rolledValue = dice.Roll();

            ResolveDice(ctx);
        }
    }

    protected virtual void ResolveDice(BattleContext ctx)
    {
        switch (ctx.currentDice.type)
        {
            case DiceType.Attack:
                ctx.target.TakeDamage(ctx.rolledValue);
                break;

            
        }
    }
}