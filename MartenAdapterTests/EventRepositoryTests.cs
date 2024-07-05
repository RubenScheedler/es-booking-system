using Domain.Events;
using Domain.Exceptions;
using FluentAssertions;
using Marten;
using MartenAdapter;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace MartenAdapterTests;

public class EventRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    private EventRepository _eventRepository;
    private DocumentStore _documentStore;

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
        
        _eventRepository = new EventRepository(_documentStore);
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void SaveEvents_EmptyCollection_ThrowsException()
    {
        // Act
        var action = () => _eventRepository.SaveEvents([]);
        
        // Assert
        action.Should().Throw<EmptyEventStreamException>();
    }
    
    [Fact]
    public void SaveEvents_StreamDoesNotExistYet_SavesEventsIntoDatabase()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var createdAt = DateTime.Now;
        IReadOnlyCollection<IBookingEvent> events = [new BookingCreated(bookingId, createdAt)];

        // Act
        _eventRepository.SaveEvents(events);
        
        // Assert
        var result = _eventRepository.GetEvents(bookingId);
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public void SaveEvents_StreamExistsAlready_ThrowsNotImplementedException()
    {
        // Arrange
        using var session = _documentStore.LightweightSession();
        var bookingId = Guid.NewGuid();
        var createdAt = DateTime.Now;
        session.Events.StartStream<IBookingEvent>(bookingId, [new BookingCreated(bookingId, createdAt)]);
        session.SaveChanges();
        
        // Act
        var action = () => _eventRepository.SaveEvents([new BookingCreated(bookingId, createdAt)]);
        
        // Assert
        action.Should().Throw<NotImplementedException>();
    }
}