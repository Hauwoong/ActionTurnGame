using UnityEngine;

[CreateAssetMenu(menuName = "Game/Passive/Max Hp Boost")]
public class MaxHpBoostPassiveData : PassiveData, IStatModifierPassive
{
    [SerializeField] private int amount;
    public override PassiveType Type => PassiveType.MaxHpBoost;
    public int Amount => amount;
    public void Apply(CharacterStateBuilder builder)
        => builder.MaxHp += Amount;
}