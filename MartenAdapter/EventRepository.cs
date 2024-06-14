using Domain.Events;
using Domain.Ports.Output;
using Marten;

namespace MartenAdapter;

public class EventRepository(IDocumentStore store) : ISaveEventsPort
{
    public void SaveEvents(IReadOnlyCollection<IEvent> events)
    {
        using var session = store.LightweightSession();

        var bookingCreated = (BookingCreated)events.First();
        session.Events.StartStream(bookingCreated.Id, events);

        session.SaveChanges();
    }

    public IReadOnlyCollection<IEvent> GetEvents(Guid aggregateId)
    {
        using var session = store.LightweightSession();

        return session.Events.FetchStream(aggregateId).Select(e => e.Data).Cast<IEvent>().ToList();
    }
}