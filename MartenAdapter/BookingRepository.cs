using Domain.Events;
using Domain.Exceptions;
using Domain.Ports.Output;
using Marten;

namespace MartenAdapter;

public class BookingRepository(IDocumentStore store) : IGetBookingEventsPort, ISaveBookingPort
{
    public IReadOnlyCollection<IBookingEvent> GetBookingEvents(Guid bookingId)
    {
        using var session = store.LightweightSession();

        return session.Events.FetchStream(bookingId).Select(e => e.Data).Cast<IBookingEvent>().ToList();
    }
    
    public void SaveBookingEvents(IReadOnlyCollection<IBookingEvent> events, long expectedVersion)
    {
        if (events.IsEmpty())
        {
            throw new EmptyEventStreamException("Cannot save empty event collection");
        }

        var aggregateId = events.First().BookingId;

        using var session = store.LightweightSession();

        session.Events.Append(aggregateId, expectedVersion, events);

        session.SaveChanges();
    }

}