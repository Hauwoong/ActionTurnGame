// Dispatcher - ±¸Á¶¸¸
using System;
using System.Collections.Generic;

public class LogDispatcher
{
    private readonly Dictionary<Type, Action<CombatLog>> _handlers = new();

    public void Register<T>(Action<T> handler) where T : CombatLog
    {
        _handlers[typeof(T)] = log => handler((T)log);
    }

    public void Dispatch(CombatLog log)
    {
        if (_handlers.TryGetValue(log.GetType(), out var handler))
            handler(log);
    }

    public void DispatchAll(IReadOnlyList<CombatLog> logs)
    {
        foreach (var log in logs)
            Dispatch(log);
    }
}
