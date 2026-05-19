public static class PassiveFactory
{
    public static PassiveEffect Create(PassiveData data, CharacterRuntime owner)
    {
        return data.Type switch
        {
            PassiveType.AttackBoost => new AttackBoostPassive(owner, ((AttackBoostData)data).Amount),
            PassiveType.SpeedSlot => new SpeedSlotPassive(owner),
            PassiveType.EmotionOnAttack => null,  // IStatModifierPassive — CharacterStateBuilder에서 처리
            PassiveType.MaxHpBoost => null,        // IStatModifierPassive — CharacterStateBuilder에서 처리
            _ => throw new System.ArgumentException($"Unknown passive type: {data.Type}")
        };
    }
}