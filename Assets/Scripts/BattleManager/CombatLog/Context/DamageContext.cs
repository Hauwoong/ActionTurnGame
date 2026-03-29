public class DamageContext : IClashContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }
    public bool IsCancelled { get; set; }

    public int BaseDamage;

    public int Additive;
    public int FinalDamage => BaseDamage + Additive;

    public DamageContext(CharacterRuntime attacker, CharacterRuntime defender, int baseDamage)
    {
        Attacker = attacker;
        Defender = defender;
        BaseDamage = baseDamage;
    }
}
