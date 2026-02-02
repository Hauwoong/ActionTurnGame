using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Game/Card")]
public class CardData : ScriptableObject
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

    public virtual bool CanUse(BattleContext ctx)
    {
        return ctx.user.currentEnergy >= cost;
    }

    public virtual void PayCost(BattleContext ctx)
    {
        ctx.user.UseEnergy(cost);
    }

    public virtual bool Use(BattleContext ctx)
    {
        if (!CanUse(ctx))
        {
            Debug.Log("Not enough energy to use this card.");
            return false;
        }
        PayCost(ctx);

        ResolveCard(ctx);

        return true;
    }

    protected virtual void ResolveCard(BattleContext ctx)
    {
        foreach (var dice in dices)
        {
            var result = new DiceResult
            {
                type = dice.type,
                value = dice.CardDiceRoll(),
                owner = ctx.currentActor
            };

            ctx.allDice.Add(result);
        }
    }
}