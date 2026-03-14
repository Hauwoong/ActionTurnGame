
public class DiceRuntime
{
    public DiceData Data { get; }
    public ActionInstance Action { get; }
    public CardData Card { get; }
    public int CurrentRoll { get; private set; }
    public bool IsDestroyed { get; private set; }

    public DiceRuntime(DiceData data, ActionInstance action)
    {
        Data = data;
        Action = action;
        IsDestroyed = false;
    }

    public void Roll(IRng rng)
    {
        CurrentRoll = rng.Range(Data.Min, Data.Max+1);
    }

    public void Destory()
    {
        IsDestroyed = true;
    }
}
