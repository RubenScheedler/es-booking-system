using Domain.Events;

namespace Domain;

public class Booking
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<IBookingEvent> _events = [];

    // For event sourced reconstruction
    public Booking(List<IBookingEvent> events)
    {
        events.ForEach(Apply);
    }
    
    public Booking(DateTime createdAt)
    {
        ApplyEvent(new BookingCreated(Guid.NewGuid(), createdAt));
    }

    private void Apply(IBookingEvent bookingEvent)
    {
        switch (bookingEvent)
        {
            case BookingCreated bookingCreated:
                ApplyEvent(bookingCreated);
                break;
            default:
                throw new Exception($"Event not supported: {bookingEvent.GetType()}");
        }
    }
    
    private void ApplyEvent(BookingCreated @event)
    {
        Id = @event.BookingId;
        CreatedAt = @event.CreatedAt;
        _events.Add(new BookingCreated(Id, CreatedAt));
    }

    public IReadOnlyCollection<IBookingEvent> GetEvents()
    {
        return _events.AsReadOnly();
    }
}