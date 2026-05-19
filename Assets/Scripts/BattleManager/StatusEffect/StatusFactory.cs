public static class StatusFactory
{
    public static StatusEffectRuntime Create(StatusEffectType type, CharacterRuntime owner, int stack)
    {
        return type switch
        {
            StatusEffectType.Bleed => new BleedEffect(owner, stack),
            StatusEffectType.Burn => new BurnEffect(owner, stack),
            StatusEffectType.Strength => new StrengthEffect(owner, stack),
            StatusEffectType.Paralysis => new ParalysisEffect(owner, stack),
            _ => throw new System.ArgumentException($"Unknown status effect: {type}")
        };
    }
}
