
public class StaggerContext : IClashContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }
    public bool IsCancelled { get; set; }
    public int BaseValue { get; }
    public int Additive {  get; }
    public int FinalValue => BaseValue + Additive;
    public bool IsRecover { get; }

    public StaggerContext(CharacterRuntime attacker, CharacterRuntime defender, int baseValue, bool isRecover)
    {
        Attacker = attacker;
        Defender = defender;
        BaseValue = baseValue;
        IsRecover = isRecover;
    }
}