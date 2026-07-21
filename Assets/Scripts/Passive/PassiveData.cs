using UnityEngine;
using UnityEngine;

public abstract class PassiveData : ScriptableObject
{
    [SerializeField] private string passiveName;
    public string Name => passiveName;
    public abstract PassiveType Type { get; }
}

public enum PassiveType
{
    SpeedSlot,
    AttackBoost,
    EmotionOnAttack,
    MaxHpBoost,
}