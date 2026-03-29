
public class DiceConsumedEvent : ICombatEvent
{
    public int CharacterId { get; }

    public DiceConsumedEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.AdvanceDice(CharacterId, AdvanceType.Consume);
    }
}