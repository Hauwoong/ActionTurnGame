
public class DiceRollContext
{
    public CharacterRuntime Owner { get; }
    public DiceRuntime Dice { get; }
    public int ModifiedRoll { get; set; }

    public DiceRollContext(CharacterRuntime owner, DiceRuntime dice)
    {
        Owner = owner;
        Dice = dice;
        ModifiedRoll = dice.CurrentRoll;
    }
}