using JetBrains.Annotations;

public sealed class DiceDestoryedEvent : ICombatEvent
{
    public DiceHandle Handle { get; private set; }

    public DiceDestoryedEvent(DiceHandle handle)
    {
        Handle = handle;
    }

    public void Apply(BattleRuntime runtime)
    {
        var dice = runtime.GetDice(Handle);
        dice.IsDestoryed = true; ;
    }
}
