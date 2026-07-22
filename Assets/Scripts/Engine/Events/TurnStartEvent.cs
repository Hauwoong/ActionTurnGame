
public class TurnStartEvent : ICombatEvent
{
    public int CharacterId { get; }
    public TurnStartEvent(int characterId)
    {
        CharacterId = characterId;
    }
    public void Apply(BattleRuntime runtime)
    {
        var character = runtime.GetCharacterRuntime(CharacterId);

        // 턴 시작시 빛 하나 회복 << 턴시작 앞이어야 하는 이유: 턴 시작 훅이 에너지를 읽거나 조작할 수 있는데, 회복 전 값을 보면 안 되기 때문에 턴 시작 앞에다가 둠
        runtime.EnqueueEvent(new EnergyRecoverEvent(CharacterId, 1));

        // 턴 시작 훅 호출
        character.TriggerTurnStart();
        runtime.AddLog(new TurnStartLog(CharacterId));

        // 턴 시작시 카드 한 장을 뽑기
        runtime.EnqueueEvent(new DrawCardEvent(CharacterId, 1));
    }
}