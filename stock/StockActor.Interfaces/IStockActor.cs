using Dapr.Actors;
using OrderActor.Interfaces;

namespace StockActor.Interfaces;

public interface IStockActor : IActor
{       
    Task<bool> ReserveStockAsync(List<LineItem> data);
    
    Task<bool> ReturnStockAsync(List<LineItem> data);
}

