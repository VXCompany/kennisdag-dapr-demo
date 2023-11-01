using TicketMasterApi.Models;
using TicketMasterApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IShoppingCartService, ShoppingCartService>();

var app = builder.Build();

app.MapGet("/", () => "Ticketmaster API is running");

app.MapPost("/users", async (IUserService userService, User user) =>
{
    var result = await userService.CreateAsync(user);
    return result ? Results.Created($"/users/{user.Username}", user) : Results.BadRequest(
        new
        {
            errorMessage = "User could not be created"
        });
});

app.MapPost("/users/{username}/cart", async (IShoppingCartService shoppingCartService, string username, Cart cart) =>
{
    var result = await shoppingCartService.CreateAsync(cart, username);
    return result ? Results.Created($"/users/{username}/cart", cart) : Results.BadRequest(
        new
        {
            errorMessage = "Cart could not be created"
        });
});

app.MapPost("/users/{username}/checkout", async (IShoppingCartService shoppingCartService, string username) =>
{
    var result = await shoppingCartService.CheckoutAsync(username);

    return result ? Results.Accepted() : Results.BadRequest(
        new
        {
            errorMessage = "Order could not be created"
        });
});

app.MapPost("/users/{username}/checkout/payorder", async (IShoppingCartService shoppingCartService, string username) =>
{
    var result = await shoppingCartService.PayOrderAsync(username);

    return result ? Results.Accepted() : Results.BadRequest(
        new
        {
            errorMessage = "Payment could not be processed"
        });
});

var DatabaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await DatabaseInitializer.InitializeAsync();

app.Run();