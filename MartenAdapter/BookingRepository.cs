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

        var eventStream = session.Events.FetchStream(aggregateId);
        if (eventStream.IsEmpty())
        {
            session.Events.StartStream(aggregateId, events);
        }
        else
        {
            throw new NotImplementedException();
            // session.Events.Append(aggregateId, events);
        }
        session.SaveChanges();
    }

    public IReadOnlyCollection<IBookingEvent> GetEvents(Guid aggregateId)
    {
        using var session = store.LightweightSession();

        return session.Events.FetchStream(aggregateId).Select(e => e.Data).Cast<IBookingEvent>().ToList();
    }
}