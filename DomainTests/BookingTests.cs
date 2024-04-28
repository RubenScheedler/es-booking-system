using Domain;
using Domain.Events;
using FluentAssertions;
using Xunit;

namespace DomainTests;

public class BookingTests
{
    [Fact]
    public void Constructor_SetsIdAndCreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var booking = new Booking(createdAt);

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Reconstruction_WithBookingCreatedEvent_SetsIdAndCreatedAt()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var bookingCreatedEvent = new BookingCreated(bookingId, createdAt);

        // Act
        var result = new Booking([bookingCreatedEvent]);

        // Assert
        result.Id.Should().Be(bookingId);
        result.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void GetEvents_ReturnsEvents()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var bookingCreatedEvent = new BookingCreated(bookingId, createdAt);
        var booking = new Booking([bookingCreatedEvent]);
        
        // Act
        var results = booking.GetEvents();
        
        // Assert
        results.Should().BeEquivalentTo([bookingCreatedEvent]);
    }
}