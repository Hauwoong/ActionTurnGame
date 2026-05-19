public class EnergyUseEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public EnergyUseEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.UseEnergy(Amount);
        runtime.AddLog(new EnergyUseLog(CharacterId, Amount));
    }
}