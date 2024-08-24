using Domain.ports.input;
using Domain.Ports.Output;

namespace Domain.UseCases;

public class RescheduleBookingUseCase(IGetBookingEventsPort getBookingEventsPort, ISaveBookingPort saveBookingPort) : IRescheduleBookingPort
{
    // TODO transaction wrapper + retry
    public void RescheduleBooking(Guid bookingId, DateTime newFrom, DateTime newTo)
    {
        var bookingEvents = getBookingEventsPort.GetBookingEvents(bookingId);
        var originalAggregateVersion = bookingEvents.Count;

        var booking = new Booking(bookingEvents);
        
        booking.Reschedule(newFrom, newTo);

        var newEvents = booking.GetNewEvents();
        var expectedVersion = originalAggregateVersion + newEvents.Count;
        saveBookingPort.SaveBookingEvents(newEvents, expectedVersion);
    }
}