using System.Linq.Expressions;
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
    private readonly Mock<IGetBookingEventsPort> _getBookingEventsPort;
    private readonly Mock<ISaveBookingPort> _saveBookingPort;

    public RescheduleBookingUseCaseTests()
    {
        _getBookingEventsPort = new Mock<IGetBookingEventsPort>();
        _getBookingEventsPort.Setup(port => port.GetBookingEvents(It.IsAny<Guid>()))
            .Returns([BookingEventsFixture.ValidBookingCreated(_bookingId)]);
        _saveBookingPort = new Mock<ISaveBookingPort>();
        _systemUnderTest = new RescheduleBookingUseCase(_getBookingEventsPort.Object, _saveBookingPort.Object);
    }

    [Fact]
    public void RescheduleBooking_RetrievesBooking()
    {
        // Act
        _systemUnderTest.RescheduleBooking(_bookingId, DateTime.Now, DateTime.Now.AddDays(1));
        
        // Assert
        _getBookingEventsPort.Verify(port => port.GetBookingEvents(_bookingId));
    }
    
    [Fact]
    public void RescheduleBooking_SavesRescheduledBooking()
    {
        // Arrange
        var newFrom = DateTime.Now;
        var newTo = DateTime.Now.AddDays(1);
        
        var expectedEvent = new BookingRescheduled(_bookingId, newFrom, newTo);
        var expectedExpectedVersion = 2; // created, rescheduled
        
        // Act
        _systemUnderTest.RescheduleBooking(_bookingId, newFrom, newTo);
        
        // Assert
        _saveBookingPort.Verify(port =>
            port.SaveBookingEvents(
                It.Is<IReadOnlyCollection<IBookingEvent>>(actual => 
                    actual.Count == 1 &&
                    actual.ToList()[0].Equals(expectedEvent)),
                It.Is<long>(actual => actual.Equals(expectedExpectedVersion))
            )
        );
    }
}