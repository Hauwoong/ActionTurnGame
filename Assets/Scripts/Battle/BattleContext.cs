using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BattleContext
{
    public Player user;
    public Enemy target;

    public Character currentActor;

    public List<DiceResult> allDice = new();

    public List<SpeedDiceResult> speedResults = new();

    // 생성자 추가
    public BattleContext(Player user, Enemy target)
    {
        this.user = user;
        this.target = target;
    }
}
public class SpeedDiceResult
{
    public Character owner;
    public int speed;
    public CardData card;

    public bool IsReady => card != null;
}

