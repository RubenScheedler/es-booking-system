using System.Data.Common;
using Domain;
using Domain.Events;
using Domain.Exceptions;
using Domain.Ports.Output;
using Marten;
using Marten.Events;

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

        var versionOfPersistedAggregate = GetAggregateVersion(aggregateId, session);

        var eventsToBePersisted = GetNewEventsSinceVersion(events, versionOfPersistedAggregate);
        var expectedNewVersion = events.Count;
        
        session.Events.Append(aggregateId, expectedNewVersion, eventsToBePersisted);

        session.SaveChanges();
    }

    /// <summary>
    /// Determines the version of the aggregate as found in persisted storage.
    /// Returns 0 if the stream of aggregate does not exist, or if the stream
    /// exists without any events in it.
    /// </summary>
    private long GetAggregateVersion(Guid aggregateId, IDocumentSession session)
    {
        StreamState? eventStreamState;
        try
        {
            eventStreamState = session.Events.FetchStreamState(aggregateId);
        }
        catch (DbException)
        {
            // This occurs when the fetchStreamState happens before any state staging
            // Marten operation. In that case, the table of stream states is not yet present
            return 0;
        }

        return eventStreamState?.Version ?? 0;
    }

    private IEnumerable<IBookingEvent> GetNewEventsSinceVersion(IReadOnlyCollection<IBookingEvent> events, long sinceVersion)
    {
        return events.Skip((int)sinceVersion);
    }

    public IReadOnlyCollection<IBookingEvent> GetEvents(Guid aggregateId)
    {
        using var session = store.LightweightSession();

        return session.Events.FetchStream(aggregateId).Select(e => e.Data).Cast<IBookingEvent>().ToList();
    }
}