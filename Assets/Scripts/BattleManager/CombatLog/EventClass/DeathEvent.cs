public class DeathEvent : ICombatEvent
{
    public int CharacterId { get; }
    public DeathEvent(int characterId)
    {
        CharacterId = characterId;
    }
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.Die();  
        runtime.AddLog(new DeathLog(CharacterId));

        if (runtime.IsBattleOver())
            runtime.EnqueueEvent(new BattleEndEvent());
    }
}