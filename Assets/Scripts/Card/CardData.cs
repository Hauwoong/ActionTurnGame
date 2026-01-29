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

        ApplyResult(ctx);

        return true;
    }

    protected virtual void ResolveCard(BattleContext ctx)
    {
        foreach (var dice in dices)
        {
            ctx.currentDice = dice;
            ctx.rolledValue = dice.Roll();

            Debug.Log($"{ctx.user.PlayerName} rolled a {ctx.rolledValue} on a {dice.type} dice.");

            ResolveDice(ctx);
        }
    }

    protected virtual void ResolveDice(BattleContext ctx)
    {
        switch (ctx.currentDice.type)
        {
            case DiceType.Attack:
                ctx.pendingDamage += ctx.rolledValue;
                break;

            case DiceType.Block:
                ctx.pendingDamage += ctx.rolledValue;
                break;
        }
    }

    protected virtual void ApplyResult(BattleContext ctx)
    {
        int finalDamage = ctx.pendingDamage - ctx.pendingGuard;

        if (finalDamage > 0)
        {
            ctx.target.TakeDamage(finalDamage);

            Debug.Log($"Damage Applied: {finalDamage}");
        }

        else
        {
            Debug.Log("All damage blocked!");
        }
    }
}