using Domain;
using Domain.ports.input;
using Domain.Ports.Output;
using Domain.UseCases;
using Domain.Utility;
using Marten;
using MartenAdapter;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("pedro")!);
// Marten
builder.Services.AddMarten().UseLightweightSessions()
    .UseNpgsqlDataSource();

// DI registration
builder.Services.AddTransient<IClock, Clock>();
builder.Services.AddTransient<ISaveEventsPort, EventRepository>();
builder.Services.AddTransient<ICreateBookingPort, CreateBookingUseCase>();

// App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Booking endpoints
app.MapPost("/booking", ([FromServices] ICreateBookingPort createBookingUseCase) => createBookingUseCase.CreateBooking()).WithOpenApi();
app.MapGet("/booking/{id:guid}", (HttpContext context, Guid id) => new NotImplementedException()).WithOpenApi();

app.Run();
