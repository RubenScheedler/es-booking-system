using Domain.Events;
using Domain.Ports.Output;
using Domain.UseCases;
using Domain.Utility;
using FluentAssertions;
using Moq;
using Xunit;

namespace DomainTests.UseCases;

public class CreateBookingUseCaseTests
{
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<ISaveEventsPort> _saveEventsPortMock;
    private readonly DateTime _now = DateTime.Now;

    public CreateBookingUseCaseTests()
    {
        _clockMock = new Mock<IClock>();
        _clockMock.Setup(clock => clock.Now()).Returns(_now);

        _saveEventsPortMock = new Mock<ISaveEventsPort>();
        _saveEventsPortMock.Setup(port => port.SaveEvents(It.IsAny<IReadOnlyCollection<IEvent>>()));
    }

    [Fact]
    public void CreateBooking_CallsClockNow()
    {
        var sut = new CreateBookingUseCase(_clockMock.Object, _saveEventsPortMock.Object);
        
        // Act
        sut.CreateBooking();
        
        // Assert
        _clockMock.VerifyAll();
        _clockMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void CreateBooking_CallsSaveEvents()
    {
        var sut = new CreateBookingUseCase(_clockMock.Object, _saveEventsPortMock.Object);
        
        // Act
        sut.CreateBooking();
        
        // Assert
        _saveEventsPortMock.VerifyAll();
        _saveEventsPortMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void CreateBooking_ReturnsNewBooking()
    {
        // Arrange
        var sut = new CreateBookingUseCase(_clockMock.Object, _saveEventsPortMock.Object);
        
        // Act
        var result = sut.CreateBooking();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().Be(_now);
    }
}