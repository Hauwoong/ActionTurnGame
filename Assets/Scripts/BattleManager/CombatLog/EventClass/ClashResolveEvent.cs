
public class ClashResolveEvent : ICombatEvent
{
    private ClashInput input;

    public ClashResolveEvent(ClashInput input)
    {
        this.input = input;
    }

    public void Apply(BattleRuntime runtime)
    {
        runtime.Executor.Execute(input);
    }
}
