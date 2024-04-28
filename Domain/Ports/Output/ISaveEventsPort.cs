using Domain.Events;

namespace Domain.Ports.Output;

public interface ISaveEventsPort
{
    void SaveEvents(IReadOnlyCollection<IEvent> events);
}