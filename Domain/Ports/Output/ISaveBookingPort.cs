using Domain.Events;

namespace Domain.Ports.Output;

public interface ISaveBookingPort
{
    void SaveBookingEvents(IReadOnlyCollection<IBookingEvent> events, long expectedVersion);
}