
public class StatusAddLog : CombatLog
{
    public int CharacterId { get; }
    public StatusEffectType Type { get; }
    public int Stack { get; }
    public StatusAddLog(int characterId, StatusEffectType type, int stack)
    {
        CharacterId = characterId;
        Type = type;
        Stack = stack;
    }
}