using Domain.ports.input;
using Domain.Ports.Output;
using Domain.Utility;

namespace Domain.UseCases;

public class CreateBookingUseCase(IClock clock, ISaveEventsPort saveEventsPort) : ICreateBookingPort
{
    public Booking CreateBooking(DateTime from, DateTime to)
    {
        var newBooking = new Booking(from, to, clock.Now());

        saveEventsPort.SaveEvents(newBooking.GetEvents());
        
        return newBooking;
    }
}