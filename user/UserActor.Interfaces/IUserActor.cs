using Dapr.Actors;

namespace UserActor.Interfaces;

public interface IUserActor : IActor
{
    Task<bool> SetShoppingCartAsync(ShoppingCart data);
    Task<ShoppingCart> GetShoppingCartAsync();
    
    Task<bool> ClearShoppingCartAsync();
}

public class ShoppingCartItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}

public class ShoppingCart
{
    public List<ShoppingCartItem> Items { get; set; }

    public override string ToString()
    {
        var numberOfItems = Items.Count;
        return $"Count: {numberOfItems}";
    }
}