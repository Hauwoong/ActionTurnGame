using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BattleContext
{
    public Player user;
    public Enemy target;

    public int rolledValue;
    public DiceData currentDice;

    public int pendingDamage;
    public int pendingGuard;

    public List<DiceResult> playerDice = new();
    public List<DiceResult> enemyDice = new();

    // 생성자 추가
    public BattleContext(Player user, Enemy target)
    {
        this.user = user;
        this.target = target;
    }
}
