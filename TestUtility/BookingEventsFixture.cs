using Domain;
using Domain.Events;

namespace TestUtility;

public static class BookingEventsFixture
{
    public static Booking ValidBooking()
    {
        return new Booking([ValidBookingCreated(Guid.NewGuid())]);
    }
    
    public static BookingCreated ValidBookingCreated(Guid bookingId)
    {
        var bookingCreated = new BookingCreated(
            bookingId, 
            DateTime.Now, 
            DateTime.Now.AddDays(1), 
            DateTime.Now
        );
        return bookingCreated;
    }
}