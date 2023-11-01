using System.Text.Json.Serialization;
using Dapr;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

app.MapSubscribeHandler();

app.MapPost("/count", [Topic("ticketpubsub", "tickets")] (object count) => {
    Console.WriteLine($"Number of tickets remaining: {count}");
    return Results.Ok(count);
});

await app.RunAsync();

public record Count([property: JsonPropertyName("count")] int count);