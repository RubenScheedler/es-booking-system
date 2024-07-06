using AutoFixture;
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
        IReadOnlyCollection<IBookingEvent> events = [_stubCreatedEvent];

        // Act
        _eventRepository.SaveEvents(events);
        
        // Assert
        var result = _eventRepository.GetEvents(_stubCreatedEvent.BookingId);
        result.Should().HaveCount(1);
    }
    
    [Fact]
    public void SaveEvents_StreamExistsAlready_ThrowsNotImplementedException()
    {
        // Arrange
        using var session = _documentStore.LightweightSession();
        session.Events.StartStream<IBookingEvent>(_stubCreatedEvent.BookingId, [_stubCreatedEvent]);
        session.SaveChanges();
        
        // Act
        var action = () => _eventRepository.SaveEvents([_stubCreatedEvent]);
        
        // Assert
        action.Should().Throw<NotImplementedException>();
    }
}