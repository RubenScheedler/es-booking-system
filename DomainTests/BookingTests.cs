using AutoFixture;
using Domain;
using Domain.Events;
using FluentAssertions;
using TestUtility;
using Xunit;

namespace DomainTests;

public class BookingTests
{
    private readonly Fixture _fixture = new ();
    private readonly DateTime _anyDate;

    public BookingTests()
    {
        _anyDate = _fixture.Create<DateTime>();
    }
    
    [Fact]
    public void Constructor_SetsId()
    {
        // Act
        var booking = new Booking(_anyDate, _anyDate, _anyDate);

        // Assert
        booking.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Constructor_SetsCreatedAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;

        // Act
        var booking = new Booking(_anyDate, _anyDate, createdAt);

        // Assert
        booking.CreatedAt.Should().Be(createdAt);
    }
    
    [Fact]
    public void Constructor_SetsFrom()
    {
        // Arrange
        var from = DateTime.UtcNow;

        // Act
        var booking = new Booking(from, _anyDate, _anyDate);

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.From.Should().Be(from);
    }
    
    [Fact]
    public void Constructor_SetsTo()
    {
        // Arrange
        var to = DateTime.UtcNow;

        // Act
        var booking = new Booking(_anyDate, to, _anyDate);

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.To.Should().Be(to);
    }

    [Fact]
    public void Constructor_AddsBookingCreatedEventToEvents()
    {
        // Act
        var booking = new Booking(_anyDate, _anyDate, _anyDate);

        // Assert
        booking.GetEvents().Should().Contain(new BookingCreated(booking.Id, booking.From, booking.To, booking.CreatedAt));
    }
    
    [Fact]
    public void Reconstruction_WithBookingCreatedEvent_SetsIdAndCreatedAt()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow;
        var createdAt = DateTime.UtcNow;
        var bookingCreatedEvent = new BookingCreated(bookingId, from, to, createdAt);

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
        var from = DateTime.UtcNow;
        var to = DateTime.UtcNow;
        var createdAt = DateTime.UtcNow;
        var bookingCreatedEvent = new BookingCreated(bookingId, from, to, createdAt);
        var booking = new Booking([bookingCreatedEvent]);
        
        // Act
        var results = booking.GetEvents();
        
        // Assert
        results.Should().BeEquivalentTo([bookingCreatedEvent]);
    }

    [Fact]
    public void Reschedule_AddsBookingRescheduledEvent()
    {
        // Arrange
        var booking = BookingFixture.ValidBooking();
        var newFrom = booking.From.AddDays(1);
        var newTo = booking.To.AddDays(1);

        // Act
        booking.Reschedule(newFrom, newTo);
        
        // Assert
        booking.GetEvents().Last().Should().Be(new BookingRescheduled(booking.Id, newFrom, newTo));
    }
    
    [Fact]
    public void Reschedule_UpdatesFrom()
    {
        // Arrange
        var booking = BookingFixture.ValidBooking();
        var newFrom = booking.From.AddDays(1);
        var newTo = booking.To.AddDays(1);

        // Act
        booking.Reschedule(newFrom, newTo);
        
        // Assert
        booking.From.Should().Be(newFrom);
    }
    
    
    [Fact]
    public void Reschedule_UpdatesTo()
    {
        // Arrange
        var booking = BookingFixture.ValidBooking();
        var newFrom = booking.From.AddDays(1);
        var newTo = booking.To.AddDays(1);

        // Act
        booking.Reschedule(newFrom, newTo);
        
        // Assert
        booking.To.Should().Be(newTo);
    }
}