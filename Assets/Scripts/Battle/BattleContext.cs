using UnityEngine;

public class BattleContext
{
    public Player user;
    public Enemy target;

    public int rolledValue;
    public DiceData currentDice;

    public int pendingDamage;
    public int pendingGuard;

    // 생성자 추가
    public BattleContext(Player user, Enemy target)
    {
        this.user = user;
        this.target = target;

        pendingDamage = 0;
        pendingGuard = 0;
    }
}
