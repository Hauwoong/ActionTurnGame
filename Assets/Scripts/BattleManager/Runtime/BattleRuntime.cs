using System.Collections.Generic;

public class BattleRuntime
{
    private readonly int Seed;

    public List<CombatLog> CombatLogs = new();

    private readonly Dictionary<int, CharacterRuntime> _characters;
    public IReadOnlyDictionary<int, CharacterRuntime> Characters => _characters;

    public IRng rng { get; private set; }

    public BattleRuntime(BattleSnapShot snapShot)
    {
        Seed = snapShot.Seed;

        rng = new DeterministicRng(Seed);

        _characters = new Dictionary<int, CharacterRuntime>();

        foreach (var state in snapShot.CharacterStates)
        {
            var runtime = new CharacterRuntime(state);

            _characters[state.CharacterId] = runtime;
        }
    }

    public CharacterRuntime GetCharacterRuntime(int characterId)
    {
        return _characters[characterId];
    }

    public DiceRuntime GetDice(DiceHandle handle)
    {
        var character = _characters[handle.Owner.CharacterId];
        return character.GetDice(handle.DiceId);
    }
}
