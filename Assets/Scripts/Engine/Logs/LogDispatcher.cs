using System;
using System.Collections.Generic;

public class LogDispatcher
{
    private readonly Dictionary<Type, List<(Delegate original, Action<CombatLog> wrapper)>> _handlers = new();

    public void Register<T>(Action<T> handler) where T : CombatLog
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<(Delegate, Action<CombatLog>)>();
            _handlers[type] = list;
        }
        list.Add((handler, log => handler((T)log)));
    }

    public void Unregister<T>(Action<T> handler) where T : CombatLog
    {
        if (!_handlers.TryGetValue(typeof(T), out var list)) return;
        list.RemoveAll(e => e.original.Equals(handler));
    }

    public void Dispatch(CombatLog log)
    {
        if (!_handlers.TryGetValue(log.GetType(), out var list)) return;
        foreach (var (_, wrapper) in list.ToArray())
            wrapper(log);
    }

    public void DispatchAll(IReadOnlyList<CombatLog> logs)
    {
        foreach (var log in logs)
            Dispatch(log);
    }
    public void Clear()
    {
        _handlers.Clear();
    }
}
