
public class StatusAddEvent : ICombatEvent
{
    public int CharacterId { get; }
    public StatusEffectType Type { get; }
    public int Stack { get; }
    public bool Delayed { get; }
    public StatusAddEvent(int characterId, StatusEffectType type, int stack, bool delayed)
    {
        CharacterId = characterId;
        Type = type;
        Stack = stack;
        Delayed = delayed;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        int resultStack = character.AddStatus(Type, Stack, Delayed);
        runtime.AddLog(new StatusAddLog(CharacterId, Type, resultStack));
    }
}