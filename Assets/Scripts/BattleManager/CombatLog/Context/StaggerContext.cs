
public class StaggerContext : IClashContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }
    public bool IsCancelled { get; set; }
    public int BaseValue { get; }
    public int Additive { get; set; } = 0;
    public int FinalValue => BaseValue + Additive;
    public bool IsHeal { get; }

    public StaggerContext(CharacterRuntime attacker, CharacterRuntime defender, int baseValue, bool isHeal)
    {
        Attacker = attacker;
        Defender = defender;
        BaseValue = baseValue;
        IsHeal = isHeal;
    }
}