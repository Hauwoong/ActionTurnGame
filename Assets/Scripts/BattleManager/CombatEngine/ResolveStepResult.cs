using System.Collections.Generic;

public readonly struct ResolveStepResult
{
    public readonly IReadOnlyList<ICombatEvent> Events;
    public readonly int NextCursorA;
    public readonly int NextCursorB;

    public ResolveStepResult(
        IReadOnlyList<ICombatEvent> events,
        int nextCursorA,
        int nextCursorB)
    {
        Events = events;
        NextCursorA = nextCursorA;
        NextCursorB = nextCursorB;
    }
}