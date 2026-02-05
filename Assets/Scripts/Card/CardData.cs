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
        return ctx.currentActor.currentEnergy >= cost;
    }

    public virtual void PayCost(BattleContext ctx)
    {
        ctx.currentActor.UseEnergy(cost);
    }

    public virtual void Use(BattleContext ctx)
    {
        if (!CanUse(ctx))
        {
            Debug.Log("Not enough energy to use this card.");
            return;
        }
        PayCost(ctx);

        ResolveCard(ctx);
    }

    protected virtual void ResolveCard(BattleContext ctx)
    {
        foreach (var dice in dices)
        {
            var result = new DiceResult
            {
                type = dice.type,
                value = dice.Roll(),
                owner = ctx.currentActor
            };

            ctx.currentActor.PushDice(result);
        }
    }
}