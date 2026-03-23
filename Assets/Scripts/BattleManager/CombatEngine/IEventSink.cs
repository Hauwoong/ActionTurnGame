public interface IEventSink
{
    void EnqueueEvent(ICombatEvent ev);
}