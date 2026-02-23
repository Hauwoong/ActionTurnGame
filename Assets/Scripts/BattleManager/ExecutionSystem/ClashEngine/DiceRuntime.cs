using UnityEngine;

public class DiceRuntime
{
    public DiceType Type;
    public int Min;
    public int Max;

    public bool IsDestoryed;
    
    public int Roll()
    {
        return Random.Range(Min, Max + 1);
    }

}
