namespace Domain.Events;

public record BookingRescheduled(Guid BookingId, DateTime From, DateTime To) : IBookingEvent;