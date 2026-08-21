using System.Collections.Generic;

// BattleSnapShot은 BattleRuntime에 넘기는 불변 입력 뭉치
// 총 4계층에서 스냅샷 계층의 경계 역할:
// CharacterData(SO 청사진, Engine 밖) -> CharacterModel(순수 청사진) -> CharacterState(스냅샷) -> CharacterRuntime(가변)
// Seed + CharacterStates가 전투 하나를 완전히 결정한다.
public class BattleSnapShot
{
    public int Seed;

    private int _nextCharacterId = 0; // CharacterId는 양 진영 통합 전역 번호(아군->적군 순으로 순번이 이어진다) / 진영 구분은 id가 아니라 Team이 담는다

    private readonly List<CharacterState> _characterStates;
    public IReadOnlyList<CharacterState> CharacterStates => _characterStates;

    /// <summary>
    /// 양 진영 CharacterModel을 CharacterState 스냅샷 리스트로 굳힌다. 아군→적군 순으로 CharacterId 부여.
    /// </summary>
    /// <param name="allies">아군 진영 청사진</param>
    /// <param name="enemies">적군 진영 청사진</param>
    /// <param name="seed">전투 결정성 시드</param>
    public BattleSnapShot(IEnumerable<CharacterModel> allies, IEnumerable<CharacterModel> enemies, int seed)
    {
        Seed = seed;

        _characterStates = new List<CharacterState>();

        AddAll(allies, Team.Ally);
        AddAll(enemies, Team.Enemy);
    }

    /// <summary>
    /// 한 진영의 청사진들을 전역 연번 CharacterId 를 붙여 CharacterState 로 만들어 리스트에 추가한다.
    /// </summary>
    /// <param name="characters">추가할 진영의 청사진들</param>
    /// <param name="team">이 진영의 소속(Ally/Enemy)</param>
    void AddAll(IEnumerable<CharacterModel> characters, Team team)
    {
        foreach (var character in characters)
        {
            int id = _nextCharacterId++;

            _characterStates.Add(new CharacterState(character, id, team));
        }
    }
}
