using System.Collections.Generic;

public class BattleRuntime
{
    private readonly Dictionary<Character, CharacterRuntime> characters = new();
    public IReadOnlyDictionary<Character, CharacterRuntime> Characters => characters;

    public BattleRuntime(IEnumerable<Character> participants)
    {
        foreach (var character in participants)
        {
            characters[character] = new CharacterRuntime(character);
        }
    }

    public CharacterRuntime GetRuntime(Character character)
    {
        return characters[character];
    }
}
