
public sealed class DiceDestroyedEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceDestroyedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.AdvanceDice(CharacterId, AdvanceType.Destroy);
    }
}
