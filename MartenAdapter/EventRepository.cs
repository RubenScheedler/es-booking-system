using Domain.Events;
using Domain.Ports.Output;
using Marten;

namespace MartenAdapter;

public class EventRepository(IDocumentStore store) : ISaveEventsPort
{
    public void SaveEvents(IReadOnlyCollection<IEvent> events)
    {
        using var session = store.LightweightSession();
        
        // TODO map
        session.Events.StartStream(events);

        session.SaveChanges();
    }
}