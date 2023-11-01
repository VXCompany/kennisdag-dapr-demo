using Dapr.Actors.Runtime;
using Dapr.Client;
using StockActor.Interfaces;
using OrderActor.Interfaces;

namespace StockActorService;

internal class StockActor : Actor, IStockActor
    {
        public StockActor(ActorHost host)
            : base(host)
        {
        }

        public async Task<bool> ReserveStockAsync(List<LineItem> data)
        {
            Console.WriteLine($"Reserving stock for {data.Count} items");
            
            var client = new DaprClientBuilder().Build();
            
            // Room for improvement: wrap in transaction
            foreach (LineItem item in data)
            {
                var (original, originalETag) = await client.GetStateAndETagAsync<int>("stockstore", "sku-123");
            
                Console.WriteLine(original + " " + originalETag);

                if (original >= item.Quantity)
                {
                    var updated = original - item.Quantity;
                    var response = await client.TrySaveStateAsync("stockstore", "sku-123", updated, originalETag, new StateOptions(){
                        Concurrency = ConcurrencyMode.FirstWrite,
                        Consistency = ConsistencyMode.Strong
                    });
                    if (response)
                    {
                        // Publish an event/message using Dapr PubSub
                        await client.PublishEventAsync("ticketpubsub", "tickets", updated);
                    }
                    return response;
                } else {
                    Console.WriteLine($"Insufficient stock: {original}");
                    return false;
                }
            } 
            return false; 
        }
        
        public async Task<bool> ReturnStockAsync(List<LineItem> data)
        {
            Console.WriteLine($"Returning stock for {data.Count} items");
            
            var client = new DaprClientBuilder().Build();
            
            // Room for improvement: wrap in transaction
            foreach (LineItem item in data)
            {
                var (original, originalETag) = await client.GetStateAndETagAsync<int>("stockstore", "sku-123");
            
                Console.WriteLine(original + " " + originalETag);

                var updated = original + item.Quantity;
                var response = await client.TrySaveStateAsync("stockstore", "sku-123", updated, originalETag, new StateOptions(){
                    Concurrency = ConcurrencyMode.FirstWrite,
                    Consistency = ConsistencyMode.Strong
                });
                if (response)
                {
                    await client.PublishEventAsync("ticketpubsub", "tickets", updated);
                }
            } 
            return false; 
        }
        
        protected override Task OnActivateAsync()
        {
            Console.WriteLine($"Activating actor id: {this.Id}");
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync()
        {
            Console.WriteLine($"Deactivating actor id: {this.Id}");
            return Task.CompletedTask;
        }
    }
