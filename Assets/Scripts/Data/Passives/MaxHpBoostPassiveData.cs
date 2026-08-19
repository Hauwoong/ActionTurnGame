using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Max Hp Boost")]
public class MaxHpBoostPassiveData : PassiveData
{
    [SerializeField] private int amount;
    public override PassiveModel ToModel() => new MaxHpBoostModel(amount);
}