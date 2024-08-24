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

    private static readonly Guid BookingId = Guid.NewGuid();

    private readonly BookingCreated _stubCreatedEvent =
        new (BookingId, DateTime.Now, DateTime.Now.AddDays(1), DateTime.Now);

    private readonly BookingRescheduled _stubBookingRescheduledEvent =
        new (BookingId, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2));


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
        // Act
        var action = () => _bookingRepository.SaveBookingEvents([], 0);
        
        // Assert
        action.Should().Throw<EmptyEventStreamException>();
    }
    
    [Fact]
    public void SaveBooking_StreamDoesNotExistYet_SavesEventsIntoDatabase()
    {
        // Arrange
        var expectedVersion = 1; // created
        
        // Act
        _bookingRepository.SaveBookingEvents([_stubCreatedEvent], expectedVersion);
        
        // Assert
        var result = _bookingRepository.GetBookingEvents(_stubCreatedEvent.BookingId);
        result.Should().HaveCount(expectedVersion);
    }
    
    [Fact]
    public void SaveBooking_StreamExistsAlready_SavesEventsIntoDatabase()
    {
        // Arrange
        using var session = _documentStore.LightweightSession();
        session.Events.StartStream<IBookingEvent>(_stubCreatedEvent.BookingId, [_stubCreatedEvent]);
        session.SaveChanges();
        var expectedVersion = 2; // created, rescheduled

        // Act
        _bookingRepository.SaveBookingEvents([_stubBookingRescheduledEvent], expectedVersion);
        
        // Assert
        var result = _bookingRepository.GetBookingEvents(_stubCreatedEvent.BookingId);
        result.Should().HaveCount(expectedVersion);
    }
}