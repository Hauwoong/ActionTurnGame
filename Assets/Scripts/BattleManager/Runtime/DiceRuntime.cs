
public class DiceRuntime
{
    public DiceData Data { get; }
    public DiceType Type => Data.Type;
    public ActionInstance Action { get; }
    public DiceState State { get; private set; }
    public int CurrentRoll { get; private set; }

    public DiceRuntime(DiceData data, ActionInstance action)
    {
        Data = data;
        Action = action;
        State = DiceState.Ready;
    }

    public void Roll(IRng rng) => CurrentRoll = rng.Range(Data.Min, Data.Max + 1);
    public void Consume() => State = DiceState.Consumed;
    public void Destroy() => State = DiceState.Destroyed;
    public void Recover() => State = DiceState.Ready;
}

public enum DiceState
{
    Ready,
    Consumed,
    Destroyed
}