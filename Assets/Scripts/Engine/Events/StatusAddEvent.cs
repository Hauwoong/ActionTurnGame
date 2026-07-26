
public class StatusAddEvent : ICombatEvent
{
    public int CharacterId { get; }
    public StatusEffectType Type { get; }
    public int Stack { get; }
    public StatusAddEvent(int characterId, StatusEffectType type, int stack)
    {
        CharacterId = characterId;
        Type = type;
        Stack = stack;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        int resultStack = character.AddStatus(Type, Stack);
        runtime.AddLog(new StatusAddLog(CharacterId, Type, resultStack));
    }
}