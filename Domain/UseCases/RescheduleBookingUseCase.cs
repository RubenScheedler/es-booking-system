using Domain.ports.input;
using Domain.Ports.Output;

namespace Domain.UseCases;

public class RescheduleBookingUseCase(IGetBookingPort getBookingPort, ISaveBookingPort saveBookingPort) : IRescheduleBookingPort
{
    public void RescheduleBooking(Guid bookingId, DateTime newFrom, DateTime newTo)
    {
        var booking = getBookingPort.GetBooking(bookingId);

        booking.Reschedule(newFrom, newTo);
        
        saveBookingPort.SaveBooking(booking);
    }
}