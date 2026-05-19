public class StatusDamageContext : IDamageContext
{
    public CharacterRuntime Attacker => Defender;
    public CharacterRuntime Defender { get; }
    public bool IsCancelled { get; set; }
    public int BaseDamage { get; }
    public int Additive { get; set; } = 0;
    public int FinalDamage => BaseDamage + Additive;

    public StatusDamageContext(CharacterRuntime defender, int baseDamage)
    {
        Defender = defender;
        BaseDamage = baseDamage;
    }
}