using System.Collections.Generic;

public class BattleSnapShot
{
    public int Seed;

    private int _nextCharacterId = 0;

    private readonly List<CharacterState> _characterStates;
    public IReadOnlyList<CharacterState> CharacterStates => _characterStates;

    public BattleSnapShot(IEnumerable<Character> allies, IEnumerable<Character> enemies, int seed)
    {
        Seed = seed;

        _characterStates = new List<CharacterState>();

        AddAll(allies, Team.Ally);
        AddAll(enemies, Team.Enemy);
    }

    void AddAll(IEnumerable<Character> characters, Team team)
    {
        foreach (var character in characters)
        {
            int id = _nextCharacterId++;

            _characterStates.Add(new CharacterState(character.CharacterData, id, team));
        }
    }
}
