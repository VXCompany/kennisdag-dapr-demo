using Dapr.Client;

public class DatabaseInitializer
{
    public async Task InitializeAsync()
    {
        Console.WriteLine("Startup up...");

        Console.WriteLine("Creating initial inventory stock");
        var client = new DaprClientBuilder().Build();
        var (original, originalETag) = await client.GetStateAndETagAsync<int>("stockstore", "sku-123");

        client.TrySaveStateAsync("stockstore", "sku-123", 1200, originalETag, new StateOptions()
        {
            Concurrency = ConcurrencyMode.FirstWrite,
            Consistency = ConsistencyMode.Strong
        }).Wait();
    }
}