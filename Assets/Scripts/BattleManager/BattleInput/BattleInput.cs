using System.Collections.Generic;

public class BattleInput
{
    public IReadOnlyList<Character> Units;
    public IReadOnlyDictionary<SpeedSlot, ActionInstance> Actions;
    public int Seed;
}
