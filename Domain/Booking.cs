using Domain.Events;

namespace Domain;

public class Booking
{
    public Guid Id { get; private set; }
    public DateTime From { get; private set; }
    public DateTime To { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Contains events of changes happened to this object since its runtime construction.
    /// </summary>
    private readonly List<IBookingEvent> _newEvents = [];

    // For event sourced reconstruction
    public Booking(List<IBookingEvent> events)
    {
        events.ForEach(Apply);
    }
    
    public Booking(DateTime from, DateTime to, DateTime createdAt)
    {
        var @event = new BookingCreated(Guid.NewGuid(), from, to, createdAt);
        _newEvents.Add(@event);
        
        ApplyEvent(@event);
    }

    private void Apply(IBookingEvent bookingEvent)
    {
        switch (bookingEvent)
        {
            case BookingCreated bookingCreated:
                ApplyEvent(bookingCreated);
                break;
            case BookingRescheduled bookingRescheduled:
                ApplyEvent(bookingRescheduled);
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
    }

    private void ApplyEvent(BookingRescheduled @event)
    {
        From = @event.From;
        To = @event.To;
    }

    public IReadOnlyCollection<IBookingEvent> GetNewEvents()
    {
        return _newEvents.AsReadOnly();
    }

    public void Reschedule(DateTime newFrom, DateTime newTo)
    {
        var @event = new BookingRescheduled(Id, newFrom, newTo);
        _newEvents.Add(@event);

        ApplyEvent(@event);
    }
}