public class DamageContext
{
    public CharacterRuntime Attacker { get; }
    public CharacterRuntime Defender { get; }

    public int Damage;

    public bool IsCancelled;

    public DamageContext(CharacterRuntime attacker, CharacterRuntime defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;
        Damage = damage;
        IsCancelled = false;
    }
}
