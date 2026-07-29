
public sealed class DiceDestroyedEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceDestroyedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var entry = runtime.PeekDice(CharacterId);
        runtime.AdvanceDice(CharacterId, AdvanceType.Destroy);
        if (entry.HasValue)
            runtime.AddLog(new DiceDestroyedLog(entry.Value.Handle));
    }
}
