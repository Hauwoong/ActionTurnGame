public class DiceConsumedEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceConsumedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var entry = runtime.PeekDice(CharacterId);
        runtime.AdvanceDice(CharacterId, AdvanceType.Consume);
        if (entry.HasValue)
            runtime.AddLog(new DiceConsumedLog(entry.Value.Handle));
    }
}