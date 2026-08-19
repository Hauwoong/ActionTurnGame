using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Attack Boost")]
public class AttackBoostData : PassiveData
{
    [SerializeField] private int amount;
    public override PassiveModel ToModel() => new AttackBoostModel(amount);
}