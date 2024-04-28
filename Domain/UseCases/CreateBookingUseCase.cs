using Domain.ports.input;
using Domain.Ports.Output;
using Domain.Utility;

namespace Domain.UseCases;

public class CreateBookingUseCase(IClock clock, ISaveEventsPort saveEventsPort) : ICreateBookingPort
{
    public Booking CreateBooking()
    {
        var newBooking = new Booking(clock.Now());

        saveEventsPort.SaveEvents(newBooking.GetEvents());
        
        return newBooking;
    }
}