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
    private readonly DateTime _from = DateTime.Now.AddDays(7);
    private readonly DateTime _to = DateTime.Now.AddDays(14);
    private readonly CreateBookingUseCase _systemUnderTest;

    public CreateBookingUseCaseTests()
    {
        _clockMock = new Mock<IClock>();
        _clockMock.Setup(clock => clock.Now()).Returns(_now);

        _saveEventsPortMock = new Mock<ISaveEventsPort>();
        _saveEventsPortMock.Setup(port => port.SaveEvents(It.IsAny<IReadOnlyCollection<IBookingEvent>>()));
        
        _systemUnderTest = new CreateBookingUseCase(_clockMock.Object, _saveEventsPortMock.Object);
    }

    [Fact]
    public void CreateBooking_CallsClockNow()
    {
        // Act
        _systemUnderTest.CreateBooking(_from, _to);
        
        // Assert
        _clockMock.VerifyAll();
        _clockMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void CreateBooking_CallsSaveEvents()
    {
        // Act
        _systemUnderTest.CreateBooking(_from, _to);
        
        // Assert
        _saveEventsPortMock.VerifyAll();
        _saveEventsPortMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public void CreateBooking_ReturnsNewBooking()
    {
        // Act
        var result = _systemUnderTest.CreateBooking(_from, _to);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().Be(_now);
        result.From.Should().Be(_from);
        result.To.Should().Be(_to);
    }
}