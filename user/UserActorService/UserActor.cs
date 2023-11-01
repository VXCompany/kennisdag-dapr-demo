using Dapr.Actors.Runtime;
using UserActor.Interfaces;

namespace UserActorService;

internal class UserActor : Actor, IUserActor
{
    public UserActor(ActorHost host)
        : base(host)
    {
    }

    /// <summary>
    ///     Set ShoppingCart into actor's private state store
    /// </summary>
    /// <param name="data">the user-defined ShoppingCart data which will be stored into state store as "shopping_cart" state</param>
    public async Task<bool> SetShoppingCartAsync(ShoppingCart data)
    {
        // Data is saved to configured state store implicitly after each method execution by Actor's runtime.
        // Data can also be saved explicitly by calling this.StateManager.SaveStateAsync();
        // State to be saved must be DataContract serializable.
        await StateManager.SetStateAsync(
            "shopping_cart", // state name
            data); // data saved for the named state "shopping_cart"

        return true;
    }

    /// <summary>
    ///     Get ShoppingCart from actor's private state store
    /// </summary>
    /// <return>the user-defined ShoppingCart which is stored into state store as "shopping_cart" state</return>
    public Task<ShoppingCart> GetShoppingCartAsync()
    {
        // Gets state from the state store.
        return StateManager.GetStateAsync<ShoppingCart>("shopping_cart");
    }

    public async Task<bool> ClearShoppingCartAsync()
    {
        Console.WriteLine($"Clearing state for actor id: {Id}");
        await StateManager.RemoveStateAsync("shopping_cart");
        return true;
    }


    /// <summary>
    ///     This method is called whenever an actor is activated.
    ///     An actor is activated the first time any of its methods are invoked.
    /// </summary>
    protected override Task OnActivateAsync()
    {
        // Provides opportunity to perform some optional setup.
        Console.WriteLine($"Activating actor id: {Id}");
        return Task.CompletedTask;
    }

    /// <summary>
    ///     This method is called whenever an actor is deactivated after a period of inactivity.
    /// </summary>
    protected override Task OnDeactivateAsync()
    {
        // Provides Opporunity to perform optional cleanup.
        Console.WriteLine($"Deactivating actor id: {Id}");
        return Task.CompletedTask;
    }
}