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

        // 감정 레벨을 올립니다.
        character.EmotionLevelUp();
        runtime.AddLog(new EmotionLevelUpLog(CharacterId, character.EmotionLevel));

        //감정 레벨업 시 빛 게이지 전부 회복 >> 현재치를 최대치로 변경 하는 것이 아닌 최대치 - 현재치 만큼 회복
        int recoverAmount = character.MaxEnergy - character.CurrentEnergy;
        if (recoverAmount > 0)
        {
            runtime.EnqueueEvent(new EnergyRecoverEvent(CharacterId, recoverAmount));
        }
    }
}