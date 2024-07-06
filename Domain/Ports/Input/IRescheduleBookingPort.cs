namespace Domain.ports.input;

public interface IRescheduleBookingPort
{
    void RescheduleBooking(Guid bookingId, DateTime newFrom, DateTime newTo);
}