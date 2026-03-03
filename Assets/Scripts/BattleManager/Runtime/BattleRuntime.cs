using System.Collections.Generic;

public class BattleRuntime
{
    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;

    public BattleRuntime(IEnumerable<Character> characters)
    {
        _characters = new Dictionary<int, CharacterRuntime>();

        foreach (var c in characters)
        {
            _characters[c.Owner.Id] = c;
        }
    }

    public CharacterRuntime GetRuntime(Character character)
    {
        return characters[character];
    }

    public DiceRuntime GetDice(DiceHandle handle)
    {
        var character = characters[handle.Owner];
        return character.GetDice(handle.DiceId);
    }

    public Character GetCharacter(Character character)
    {
        var runtime = characters[character];
        return runtime.GetCharacter(character);
    }
}
