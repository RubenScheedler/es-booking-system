namespace Domain.Events;

public interface IBookingEvent
{
    Guid BookingId { get; }
}