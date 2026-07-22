
public class EnergyRecoverEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public EnergyRecoverEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.RecoverEnergy(Amount);
        runtime.AddLog(new EnergyRecoverLog(CharacterId, Amount));
    }
}