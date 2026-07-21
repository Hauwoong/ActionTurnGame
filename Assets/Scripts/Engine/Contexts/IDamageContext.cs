public interface IDamageContext : IClashContext
{
    CharacterRuntime Attacker { get; }
    int BaseDamage { get; }
    int Additive { get; set; }
    int FinalDamage { get; }
}
