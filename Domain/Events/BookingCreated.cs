namespace Domain.Events;

public record BookingCreated(Guid Id, DateTime CreatedAt) : IEvent;