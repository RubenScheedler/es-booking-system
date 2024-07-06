namespace Domain.Events;

public record BookingCreated(
    Guid BookingId, 
    DateTime From,
    DateTime To,
    DateTime CreatedAt
    ) : IBookingEvent;