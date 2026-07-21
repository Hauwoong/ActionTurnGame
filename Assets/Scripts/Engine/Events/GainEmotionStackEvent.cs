public class GainEmotionStackEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Amount { get; }

    public GainEmotionStackEvent(int characterId, int amount)
    {
        CharacterId = characterId;
        Amount = amount;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.GainEmotionStack(Amount);
        runtime.AddLog(new EmotionStackLog(CharacterId, Amount));
    }
}