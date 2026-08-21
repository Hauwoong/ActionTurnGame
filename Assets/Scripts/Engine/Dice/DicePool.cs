using System.Collections.Generic;

// 캐릭터 한명의 주사위 큐. 순서는 [이번 카드 주사위] + [저장분]이고 커서는 항상 앞에서 시작한다.
//
// 핵심 불변량 - 커서는 절대 뒤로 가지 않는다. 그래서 "같은 bout에서 저장한 걸 또 집는" 무한 루프가 구조적으로 불가능하고, Peek이 단순 전진 루프 하나로 끝난다.
// 딸린 규칙 둘: "bout 밖에서는 커서가 0" / "bout 종료 시 커서는 항상 끝".
// 둘 다 지금은 결과적으로 성립할 뿐 강제되지 않는다 - 깨지면 증상이 원인과 멀다(각 메서드 주석 참고).
public class DicePool
{
    private readonly List<DiceEntry> _dice = new();

    private int _cursor;

    /// <summary>
    /// bout 시작에 이번 카드의 주사위를 커서 위치에 끼워 넣는다.
    /// </summary>
    /// <param name="entries">적재할 주사위들. InsertRange 한 번으로 넣어야 한다 - 하나씩 Insert 하면 역순이 된다</param>
    // 불변량 의존: bout 시작에 _cursor == 0. 깨지면 새 주사위가 리스트 중간에 꽂히는데,
    // 증상이 "새 주사위가 저장분 뒤에 실림"으로 보여 원인을 엉뚱한 데서 찾게 된다.
    public void Inject(List<DiceEntry> entries) => _dice.InsertRange(_cursor, entries);

    /// <summary>
    /// 커서부터 전진하며 아직 쓸 수 있는 주사위(Ready/Used/Stored)를 하나 돌려준다. 없으면 null.
    /// </summary>
    /// <returns>쓸 수 있는 주사위. 큐가 소진됐으면 null</returns>
    // 주의 : 순수 읽기가 아니다. 소비·파괴된 주사위를 만나면 _cursor 를 밀고 지나간다.
    // 그래서 호출부는 생존 가드를 통과한 뒤에 불러야 한다 - 죽은 대상의 풀을 매 루프 건드리게 된다.
    public DiceEntry? Peek()
    {
        while (_cursor < _dice.Count)
        {
            var state = _dice[_cursor].Dice.State;
            if (state == DiceState.Ready || state == DiceState.Used || state == DiceState.Stored)
                return _dice[_cursor];
            _cursor++;
        }
        return null;
    }

    /// <summary>
    /// 커서의 주사위를 해석 결과대로 처리한다. Consume/Destroy는 커서를 전진시키고 Reuse는 제자리에 둔다.
    /// </summary>
    /// <param name="type">해석 결과. Reuse는 반격·회피 승리라 같은 주사위를 다시 굴린다</param>
    public void Advance(AdvanceType type)
    {
        // 경계 가드. Peek이 null을 준 뒤에도 DiceDestroyedEvent가 AdvanceDice를 무조건 부른다.
        if (_cursor < 0 || _cursor >= _dice.Count) return;

        var entry = _dice[_cursor];
        switch (type)
        {
            case AdvanceType.Consume:
                entry.Dice.Consume();
                _cursor++;
                break;
            case AdvanceType.Destroy:
                entry.Dice.Destroy();
                _cursor++;
                break;
            case AdvanceType.Reuse:
                entry.Dice.Use();
                break;
        }
    }

    /// <summary>
    /// 합(bout) 하나가 끝날 때의 정리. 살아남은 주사위를 저장분으로 돌리고, 파괴분을 목록에서 빼고, 커서를 0으로 되돌린다.
    /// </summary>
    // 멱등이여야 한다. 합에서 방어자가 곧 TargetId라 BoutEndEvent가 같은 캐릭터를 두 번 부르는 것에 기대고 있다.
    public void EndBout()
    {
        StoreSurvivors();
        ClearDestroyed();
        _cursor = 0;
    }

    /// <summary>
    /// 이번 합에서 굴리지 않았거나 재사용된 주사위(Consumed/Used)를 저장분(Stored)으로 돌린다.
    /// </summary>
    // Ready는 일부러 안 건드린다. "Destroyed 아니면 전부 Stored"로 총함수를 만들면 짧지만
    // "bout 종료 시 커서는 항상 끝" 위반을 조용히 덮는다 - 명시적으로 나열해야 남은 Ready가 다음 bout의 Peek에 걸려 증상으로 드러난다.
    void StoreSurvivors()
    {
        foreach (var e in _dice)
            if (e.Dice.State == DiceState.Consumed || e.Dice.State == DiceState.Used)
                e.Dice.Store();
    }

    /// <summary>
    /// 파괴된 주사위를 목록에서 제거한다. 이력은 여기가 아니라 CombatLogs에 남는다.
    /// </summary>
    void ClearDestroyed() => _dice.RemoveAll(e => e.Dice.State == DiceState.Destroyed);

    /// <summary>
    /// 턴이 끝날 때 큐를 통째로 비운다. 저장분은 턴을 넘기지 않는다 - 이 메서드의 존재 이유가 그것 하나다.
    /// </summary>
    // 두 줄은 짝이다. _cursor = 0은 _dice를 비웠다는 보장 위에서만 안전하다.
    public void ResetForNextTurn()
    {
        _dice.Clear();
        _cursor = 0;
    }

    /// <summary>
    /// 캐릭터가 죽었을 때 커서부터 끝까지 전부 소멸시킨다. 저장분도 함께 사라진다.
    /// </summary>
    // 커서를 _dice.Count까지 미는 것이 필수다. 안 밀면 Inject가 소멸된 주사위 앞에 꽂힌다.
    public void DestroyRemaining()
    {
        for (int i = _cursor; i < _dice.Count; i++)
        {
            _dice[i].Dice.Destroy();

            _cursor++;
        }
    }

    /// <summary>
    /// 합이 중단됐을 때(대상 사망 등) 커서부터 끝까지 정리한다. 공격 주사위만 소멸하고 나머지는 저장분으로 남는다.
    /// </summary>
    public void DiscardRemaining()
    {
        for (int i = _cursor; i < _dice.Count; i++)
        {
            var dice = _dice[i].Dice;

            // 여기서 Attack만 보는 것은 저장 규칙 때문이지 "공격형인가" 판정이 아니다.
            // 원작 규칙 : Block/Evade/Counter는 맞붙을 상대가 없으면 굴리지 않고 저장된다
            // 따라서 IsOffensive()로 바꾸면 안 된다. 그건 Attack|Counter라 카운터가 소멸해버린다.
            if (dice.Type == DiceType.Attack)
                dice.Destroy();

            else
                dice.Consume();

             _cursor++;
        }
    }
}

public enum AdvanceType
{
    Consume,
    Destroy,
    Reuse
}