using Dapr.Actors;
using Dapr.Actors.Client;
using OrderActor.Interfaces;
using TicketMasterApi.Models;
using UserActor.Interfaces;

namespace TicketMasterApi.Services;

public class ShoppingCartService : IShoppingCartService
{
    public async Task<bool> CreateAsync(Cart cart, string username)
    {
        if (cart is null || string.IsNullOrWhiteSpace(username) || cart.Items.Count == 0)
        {
            return false;
        }

        // Registered Actor Type in Actor Service
        var actorType = "UserActor";

        // An ActorId uniquely identifies an actor instance
        // If the actor matching this id does not exist, it will be created
        var actorId = new ActorId(username);

        // Create the local proxy by using the same interface that the service implements.
        // You need to provide the type and id so the actor can be located. 
        var proxyUser = ActorProxy.Create<IUserActor>(actorId, actorType);

        // Now you can use the actor interface to call the actor's methods.
        Console.WriteLine($"Adding items of a product to ShoppingCart on {actorType}:{actorId}...");

        // Create a ShoppingCart from a Cart
        var shoppingCart = new ShoppingCart
        {
            Items = cart.Items.Select(item => new ShoppingCartItem
            {
                ProductName = item.ProductName,
                Quantity = item.Quantity
            }).ToList()
        };

        var response = await proxyUser.SetShoppingCartAsync(shoppingCart);
        Console.WriteLine($"Got response: {response}");

        return true;
    }

    public async Task<bool> CheckoutAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        var actorType = "UserActor";
        var actorId = new ActorId(username);

        Console.WriteLine($"Creating an Order from ShoppingCart on {actorType}:{actorId}...");
        var proxyUser = ActorProxy.Create<IUserActor>(actorId, actorType);

        Console.WriteLine($"Calling GetShoppingCartAsync on {actorType}:{actorId}...");
        var savedData = await proxyUser.GetShoppingCartAsync();
        Console.WriteLine($"Got response: {savedData}");

        actorType = "OrderActor";
        var proxyOrder = ActorProxy.Create<IOrderActor>(actorId, actorType);
        Console.WriteLine($"Calling ProcessOrderAsync on {actorType}:{actorId}...");

        var order = new Order
        {
            OrderNumber = Guid.NewGuid().ToString(),
            Username = username,
            State = OrderState.New,
            Lines = savedData.Items.Select(item => new LineItem
            {
                Sku = "sku-123",
                ProductName = item.ProductName,
                Quantity = item.Quantity
            }).ToList()
        };

        var orderResponse = await proxyOrder.ProcessOrderAsync(order);
        
        Console.WriteLine($"Got response: {orderResponse}");

        if (orderResponse)
        {
            // Reminders are persistent, timers are not
            // Real world: reminders are a better fit here!
            await proxyOrder.RegisterTimer();
        }

        return true;
    }
    
    public async Task<bool> PayOrderAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return false;
        }

        var actorType = "OrderActor";
        var actorId = new ActorId(username);
        var proxyOrder = ActorProxy.Create<IOrderActor>(actorId, actorType);
        Console.WriteLine($"Calling PayOrderAsync on {actorType}:{actorId}...");

        var orderResponse = await proxyOrder.PayOrderAsync();
        
        Console.WriteLine($"Got response: {orderResponse}");

        return true;
    }
}