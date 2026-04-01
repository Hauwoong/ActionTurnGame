using System.Collections.Generic;

public class TurnStartContext
{
    public CharacterRuntime Owner { get; }
    public int EmotionLevel { get; }

    public TurnStartContext(CharacterRuntime owner)
    {
        Owner = owner;
        EmotionLevel = owner.EmotionLevel;
    }
}
