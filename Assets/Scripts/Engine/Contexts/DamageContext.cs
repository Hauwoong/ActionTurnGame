
public class DamageContext : IDamageContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }
    public bool IsCancelled { get; set; }
    public int BaseDamage {  get; }
    public int Additive { get; set; }
    public int FinalDamage => BaseDamage + Additive;

    public DamageContext(CharacterRuntime attacker, CharacterRuntime defender, int baseDamage)
    {
        Attacker = attacker;
        Defender = defender;
        BaseDamage = baseDamage;
    }
}