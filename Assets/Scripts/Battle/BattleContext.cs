using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BattleContext
{
    public Character currentActor;
    public Character target;
    // 생성자 추가
    public BattleContext(Character currentActor, Character target)
    {
        this.currentActor = currentActor;
        this.target = target;
    }
}
public struct ActionSlot
{
    public Character owner;
    public Character target;
    public CardData card;
    public int speed;
    public int diceIndex;
}

