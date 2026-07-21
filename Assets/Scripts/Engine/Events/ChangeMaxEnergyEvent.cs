public class ChangeMaxEnergyEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public ChangeMaxEnergyEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.ChangeMaxEnergy(Amount);
        runtime.AddLog(new ChangeMaxEnergyLog(CharacterId, Amount));
    }
}