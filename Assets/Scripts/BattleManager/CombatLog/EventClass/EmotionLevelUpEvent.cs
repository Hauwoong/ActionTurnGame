
public class EmotionLevelUpEvent : ICombatEvent
{
    public int CharacterId;

    public EmotionLevelUpEvent(int characterId)
    {
        CharacterId = characterId;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.Level
    }
}