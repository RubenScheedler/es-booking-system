using Domain.Events;

namespace Domain;

public class Booking
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<IEvent> _events = [];

    // For event sourced reconstruction
    public Booking(List<IEvent> events)
    {
        events.ForEach(Apply);
    }
    
    public Booking(DateTime createdAt)
    {
        ApplyEvent(new BookingCreated(Guid.NewGuid(), createdAt));
    }

    private void Apply(IEvent @event)
    {
        switch (@event)
        {
            case BookingCreated bookingCreated:
                ApplyEvent(bookingCreated);
                break;
            default:
                throw new Exception($"Event not supported: {@event.GetType()}");
        }
    }
    
    private void ApplyEvent(BookingCreated @event)
    {
        Id = @event.Id;
        CreatedAt = @event.CreatedAt;
        _events.Add(new BookingCreated(Id, CreatedAt));
    }

    public IReadOnlyCollection<IEvent> GetEvents()
    {
        return _events.AsReadOnly();
    }
}