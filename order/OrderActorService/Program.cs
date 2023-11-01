using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddActors(options =>
{
    // Register actor types and configure actor settings
    var jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
    options.JsonSerializerOptions = jsonSerializerOptions;
    
    options.Actors.RegisterActor<OrderActorService.OrderActor>();
});

var app = builder.Build();

app.MapActorsHandlers();

app.Run();