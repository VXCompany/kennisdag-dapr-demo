using Dapr.Actors;

namespace OrderActor.Interfaces;

public interface IOrderActor : IActor
{       
    Task<bool> ProcessOrderAsync(Order order);
    
    Task<bool> PayOrderAsync();
    
    Task<Order> GetOrderAsync();
    Task RegisterTimer();
    Task UnregisterTimer();
}

public class LineItem
{
    public string Sku { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    public string OrderNumber { get; set; }
    
    public string Username { get; set; }
    
    public OrderState State { get; set; }
    public List<LineItem> Lines { get; set; }

    public override string ToString()
    {
        var numberOfItems = Lines.Count;
        return $"Count: {numberOfItems}";
    }
}

public enum OrderState
{
    New,
    Accepted,
    Cancelled,
    Paid
}