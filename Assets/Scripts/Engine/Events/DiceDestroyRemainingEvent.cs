
public class DiceDestroyRemainingEvent : ICombatEvent
{
    public int CharacterId { get; }
    public DiceDestroyRemainingEvent(int characterId)
    {
        CharacterId = characterId;
    }
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);
        character.DestroyRemainingDice();
        //소비자가 아직 없고, 바로 뒤 BoutEndLog가 갱신 신호 역할을 하기에 로그가 없다
    }
}