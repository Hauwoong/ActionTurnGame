public sealed class DiceDestoryedEvent : ICombatEvent
{
    public readonly Character Owner;
    public readonly int DiceId;

    public DiceDestoryedEvent(Character owner, int diceId)
    {
        Owner = owner;
        DiceId = diceId;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.GetRuntime(Owner).MarkDestoryed(DiceId);
    }
}
