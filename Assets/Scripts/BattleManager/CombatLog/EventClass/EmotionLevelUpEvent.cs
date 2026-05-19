public class EmotionLevelUpEvent : ICombatEvent
{
    public int CharacterId { get; }

    public EmotionLevelUpEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.EmotionLevelUp();
        runtime.AddLog(new EmotionLevelUpLog(CharacterId, character.EmotionLevel));
    }
}