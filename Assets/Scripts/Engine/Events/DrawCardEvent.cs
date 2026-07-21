public class DrawCardEvent : ICombatEvent
{
    public int CharacterId { get; }
    public int Count { get; }

    public DrawCardEvent(int characterId, int count)
    {
        CharacterId = characterId;
        Count = count;
    }

    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        int drawn = character.CardManager.DrawMultiple(Count, runtime.Rng);
        runtime.AddLog(new DrawCardLog(CharacterId, drawn));
    }
}