
public class DiceReusedEvent : ICombatEvent
{
    public int CharacterId { get; }
    public DiceReusedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var entry = runtime.PeekDice(CharacterId);
        runtime.AdvanceDice(CharacterId, AdvanceType.Reuse);
        if (entry.HasValue)
            runtime.AddLog(new DiceReusedLog(entry.Value.Handle));
    }
}
