using System.Collections.Generic;
using UnityEditor.UI;

public class BattleRuntime
{
    private readonly int Seed;

    private BattleState state;

    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;

    private int NextCharacterId = 0;

    public BattleRuntime(IEnumerable<Character> characters)
    {
        _characters = new Dictionary<int, CharacterRuntime>();

        foreach (var character in characters)
        {
            int id = NextCharacterId++;

            var state = new CharacterState(character);
            var runtime = new CharacterRuntime(state,id);
            _characters.Add(id, runtime);
        }
    }

    public CharacterRuntime GetRuntime(int characterId)
    {
        return _characters[characterId];
    }

    public DiceRuntime GetDice(DiceHandle handle)
    {
        var character = _characters[handle.OwnerId];
        return character.GetDice(handle.DiceId);
    }

    public CharacterRuntime GetCharacter(int characterId)
    {
        return _characters[characterId];
    }
}
