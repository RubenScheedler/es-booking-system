using Domain.Events;
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
    
    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        
        // Configure MartenDB
        var connectionString = _postgresContainer.GetConnectionString();
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection(connectionString);
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });
        
        _eventRepository = new EventRepository(documentStore);
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync().AsTask();
    }
    
    [Fact]
    public void SaveEvents_BookingCreated_SavesEventsIntoDatabase()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var createdAt = DateTime.Now;
        IReadOnlyCollection<IEvent> events = [new BookingCreated(bookingId, createdAt)];

        // Act
        _eventRepository.SaveEvents(events);
        
        // Assert
        var result = _eventRepository.GetEvents(bookingId);
        result.Should().HaveCount(1);
    }
}