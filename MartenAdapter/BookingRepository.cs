using Domain;
using Domain.Events;
using Domain.Exceptions;
using Domain.Ports.Output;
using Marten;

namespace MartenAdapter;

public class BookingRepository(IDocumentStore store) : ISaveBookingPort
{
    public void SaveBooking(Booking booking)
    {
        var events = booking.GetEvents();
        if (events.IsEmpty())
        {
            throw new EmptyEventStreamException("Cannot save empty event collection");
        }

        var aggregateId = events.First().BookingId;
        
        using var session = store.LightweightSession();

        var eventStreamState = session.Events.FetchStreamState(aggregateId);
        if (eventStreamState == null)
        {
            session.Events.StartStream(aggregateId, events);
        }
        else
        {
            var versionOfPersistedAggregate = eventStreamState.Version;
            var eventsToBePersisted = GetNewEvents(events, versionOfPersistedAggregate);
            var expectedNewVersion = events.Count;
            
            session.Events.Append(aggregateId, expectedNewVersion, eventsToBePersisted);
        }
        session.SaveChanges();
    }

    private IEnumerable<IBookingEvent> GetNewEvents(IReadOnlyCollection<IBookingEvent> events, long sinceVersion)
    {
        return events.Skip((int)sinceVersion);
    }

    public IReadOnlyCollection<IBookingEvent> GetEvents(Guid aggregateId)
    {
        using var session = store.LightweightSession();

        return session.Events.FetchStream(aggregateId).Select(e => e.Data).Cast<IBookingEvent>().ToList();
    }
}