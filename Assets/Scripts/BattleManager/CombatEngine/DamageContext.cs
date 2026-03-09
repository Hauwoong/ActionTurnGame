public class DamageContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }

    public int BaseDamage;

    public int Additive;

    public bool IsCancelled;

    public DamageContext(CharacterRuntime attacker, CharacterRuntime defender, int baseDamage)
    {
        Attacker = attacker;
        Defender = defender;
        BaseDamage = baseDamage;
        IsCancelled = false;
    }

    public int FinalDamage
    {
        get
        {
            int value = BaseDamage + Additive;
            return value;
        }
    }
}
