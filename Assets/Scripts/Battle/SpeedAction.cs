using UnityEngine;

public class SpeedAction
{
    public Character owner;
    public int speedValue;
    public CardData card;

    public SpeedAction(Character owner, int speed, CardData card)
    {
        this.owner = owner;
        this.speedValue = speed;
        this.card = card;
    }
}