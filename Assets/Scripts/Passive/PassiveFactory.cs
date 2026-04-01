
public static class PassiveFactory
{
    public static PassiveEffect Create(PassiveData data, CharacterRuntime owner)
    {
        return data.Type switch
        {
            PassiveType.AttackBoost => new AttackBoostPassive(owner, ((AttackBoostData)data).Amount),
            PassiveType.SpeedSlot => new SpeedSlotPassive(owner),
            _ => throw new System.ArgumentException($"Unknown passive type: {data.Type}")
        };
    }
}