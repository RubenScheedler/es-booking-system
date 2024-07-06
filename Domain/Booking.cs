using Domain.Events;

namespace Domain;

public class Booking
{
    public Guid Id { get; private set; }
    public DateTime From { get; private set; }
    public DateTime To { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<IBookingEvent> _events = [];

    // For event sourced reconstruction
    public Booking(List<IBookingEvent> events)
    {
        events.ForEach(Apply);
    }
    
    public Booking(DateTime from, DateTime to, DateTime createdAt)
    {
        ApplyEvent(new BookingCreated(Guid.NewGuid(), from, to, createdAt));
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
        From = @event.From;
        To = @event.To;
        
        _events.Add(@event);
    }

    private void ApplyEvent(BookingRescheduled @event)
    {
        From = @event.From;
        To = @event.To;
        _events.Add(@event);
    }

    public IReadOnlyCollection<IBookingEvent> GetEvents()
    {
        return _events.AsReadOnly();
    }

    public void Reschedule(DateTime newFrom, DateTime newTo)
    {
        ApplyEvent(new BookingRescheduled(Id, newFrom, newTo));
    }
}