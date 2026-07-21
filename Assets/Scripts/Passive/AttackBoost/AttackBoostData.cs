using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Attack Boost")]
public class AttackBoostData : PassiveData
{
    [SerializeField] private int amount;
    public override PassiveType Type => PassiveType.AttackBoost;
    public int Amount => amount;
}