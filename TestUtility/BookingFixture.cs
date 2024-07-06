using Domain;
using Domain.Events;

namespace TestUtility;

public static class BookingFixture
{
    public static Booking ValidBooking()
    {
        return ValidBooking(Guid.NewGuid());
    }
    
    public static Booking ValidBooking(Guid bookingId)
    {
        var bookingCreated = new BookingCreated(
            bookingId, 
            DateTime.Now, 
            DateTime.Now.AddDays(1), 
            DateTime.Now
        );
        return new Booking([bookingCreated]);
    }
}