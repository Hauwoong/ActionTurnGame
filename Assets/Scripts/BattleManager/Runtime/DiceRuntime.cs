
public class DiceRuntime
{
    public DiceData Data { get; }
    public ActionInstance Action { get; }
    public DiceHandle Handle { get; }
    public int CurrentRoll { get; private set; }
    public bool IsDestroyed { get; private set; }

    public DiceRuntime(DiceData data, ActionInstance action, DiceHandle handle)
    {
        Data = data;
        Action = action;
        Handle = handle;
        IsDestroyed = false;
    }

    public void Roll(IRng rng)
    {
        CurrentRoll = rng.Range(Data.Min, Data.Max+1);
    }

    public DiceType GetDiceType()
    {
        return Data.Type;
    }

    public void Destroy()
    {
        IsDestroyed = true;
    }
}
