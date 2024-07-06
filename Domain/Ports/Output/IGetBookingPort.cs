namespace Domain.Ports.Output;

public interface IGetBookingPort
{
    Booking GetBooking(Guid bookingId);
}