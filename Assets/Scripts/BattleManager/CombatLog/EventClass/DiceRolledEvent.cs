public struct DiceRolledEvent : ICombatEvent
{
    public Character A;
    public Character B;
    public int RollA;
    public int RollB;

    public DiceRolledEvent(Character a, Character b, int rollA, int rollB)
    {
        A = a;
        B = b;
        RollA = rollA;
        RollB = rollB;
    }

    public void Apply()
}