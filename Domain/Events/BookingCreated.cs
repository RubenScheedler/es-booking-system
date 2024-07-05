namespace Domain.Events;

public record BookingCreated(Guid BookingId, DateTime CreatedAt) : IBookingEvent;