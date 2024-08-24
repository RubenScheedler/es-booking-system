using Domain.Events;

namespace Domain.Ports.Output;

public interface IGetBookingEventsPort
{
    IReadOnlyCollection<IBookingEvent> GetBookingEvents(Guid bookingId);
}