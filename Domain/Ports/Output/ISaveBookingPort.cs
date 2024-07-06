using Domain.Events;

namespace Domain.Ports.Output;

public interface ISaveBookingPort
{
    void SaveBooking(Booking booking);
}