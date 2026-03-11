
public class DiceRuntime
{
    public DiceData Data { get; }
    public int CurrentRoll { get; private set; }
    public bool IsDestroyed { get; private set; }

    public DiceRuntime(DiceData data)
    {
        Data = data;
        IsDestroyed = false;
    }

    public void Roll(IRng rng)
    {
        CurrentRoll = rng.Range(Data.min, Data.max+1);
    }

    public void Destory()
    {
        IsDestroyed = true;
    }
}
