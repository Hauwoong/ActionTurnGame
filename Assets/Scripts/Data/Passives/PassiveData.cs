using UnityEngine;

public abstract class PassiveData : ScriptableObject
{
    [SerializeField] private string passiveName;
    public string Name => passiveName;
    public abstract PassiveModel ToModel();
}