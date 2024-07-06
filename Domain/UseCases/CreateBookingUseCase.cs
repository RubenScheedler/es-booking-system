using Domain.ports.input;
using Domain.Ports.Output;
using Domain.Utility;

namespace Domain.UseCases;

public class CreateBookingUseCase(IClock clock, ISaveBookingPort saveBookingPort) : ICreateBookingPort
{
    public Booking CreateBooking(DateTime from, DateTime to)
    {
        var newBooking = new Booking(from, to, clock.Now());

        saveBookingPort.SaveBooking(newBooking);
        
        return newBooking;
    }
}