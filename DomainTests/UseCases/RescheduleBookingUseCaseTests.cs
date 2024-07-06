using Domain;
using Domain.Events;
using Domain.Ports.Output;
using Domain.UseCases;
using Moq;
using TestUtility;
using Xunit;

namespace DomainTests.UseCases;

public class RescheduleBookingUseCaseTests
{
    private readonly Guid _bookingId = Guid.NewGuid();
    private readonly RescheduleBookingUseCase _systemUnderTest;
    private readonly Mock<IGetBookingPort> _getBookingPort;
    private readonly Mock<ISaveBookingPort> _saveBookingPort;

    public RescheduleBookingUseCaseTests()
    {
        _getBookingPort = new Mock<IGetBookingPort>();
        var stubBooking = BookingFixture.ValidBooking(_bookingId);
        _getBookingPort.Setup(port => port.GetBooking(It.IsAny<Guid>()))
            .Returns(stubBooking);
        _saveBookingPort = new Mock<ISaveBookingPort>();
        _systemUnderTest = new RescheduleBookingUseCase(_getBookingPort.Object, _saveBookingPort.Object);
    }

    [Fact]
    public void RescheduleBooking_RetrievesBooking()
    {
        // Act
        _systemUnderTest.RescheduleBooking(_bookingId, DateTime.Now, DateTime.Now.AddDays(1));
        
        // Assert
        _getBookingPort.Verify(port => port.GetBooking(_bookingId));
    }
    
    [Fact]
    public void RescheduleBooking_SavesRescheduledBooking()
    {
        // Act
        var newFrom = DateTime.Now;
        var newTo = DateTime.Now.AddDays(1);
        _systemUnderTest.RescheduleBooking(_bookingId, newFrom, newTo);
        
        // Assert
        _saveBookingPort.Verify(port => port.SaveBooking(It.Is<Booking>(
            saved => saved.GetEvents().Last().Equals(new BookingRescheduled(_bookingId, newFrom, newTo)))
            )
        );
    }
}