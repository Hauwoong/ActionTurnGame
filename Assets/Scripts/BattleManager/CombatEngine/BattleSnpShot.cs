using System.Collections.Generic;

public class BattleSnapShot 
{
    public int Seed;

    public int CharacterId = 0;

    private readonly List<CharacterState> _characterStates;
    public IReadOnlyList<CharacterState> CharacterStates => _characterStates;

    public BattleSnapShot(IEnumerable<Character> characters, int seed)
    {
        Seed = seed;

        _characterStates = new List<CharacterState>();

        foreach (var character in characters)
        {
            int id = CharacterId++;

            _characterStates.Add(new CharacterState(character, id));
        }

    }    
}
