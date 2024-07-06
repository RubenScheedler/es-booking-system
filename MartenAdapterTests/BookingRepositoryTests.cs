using Domain;
using Domain.Events;
using Domain.Exceptions;
using FluentAssertions;
using Marten;
using MartenAdapter;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace MartenAdapterTests;

public class BookingRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    private BookingRepository _bookingRepository;
    private DocumentStore _documentStore;

    private readonly BookingCreated _stubCreatedEvent =
        new BookingCreated(Guid.NewGuid(), DateTime.Now, DateTime.Now.AddDays(1), DateTime.Now);

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        
        // Configure MartenDB
        var connectionString = _postgresContainer.GetConnectionString();
        _documentStore = DocumentStore.For(options =>
        {
            options.Connection(connectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });
        
        _bookingRepository = new BookingRepository(_documentStore);
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void SaveBooking_EmptyCollection_ThrowsException()
    {
        // Arrange
        var booking = new Booking([]);
        
        // Act
        var action = () => _bookingRepository.SaveBooking(booking);
        
        // Assert
        action.Should().Throw<EmptyEventStreamException>();
    }
    
    [Fact]
    public void SaveBooking_StreamDoesNotExistYet_SavesEventsIntoDatabase()
    {
        // Arrange
        var booking = new Booking([_stubCreatedEvent]);
        
        // Act
        _bookingRepository.SaveBooking(booking);
        
        // Assert
        var result = _bookingRepository.GetEvents(_stubCreatedEvent.BookingId);
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public void SaveBooking_StreamExistsAlready_ThrowsNotImplementedException()
    {
        // Arrange
        using var session = _documentStore.LightweightSession();
        session.Events.StartStream<IBookingEvent>(_stubCreatedEvent.BookingId, [_stubCreatedEvent]);
        session.SaveChanges();
        var booking = new Booking([_stubCreatedEvent]);
        
        // Act
        var action = () => _bookingRepository.SaveBooking(booking);
        
        // Assert
        action.Should().NotThrow<NotImplementedException>();
    }
}