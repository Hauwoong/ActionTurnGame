public interface ICombatEvent
{
    void Apply(BattleRuntime runtime);
}

public interface IResolutionEvent
{
    void Dispatch(BattleResolutionLoop loop);
}