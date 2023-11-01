using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using OrderActor.Interfaces;
using StockActor.Interfaces;
using UserActor.Interfaces;

namespace OrderActorService;

internal class OrderActor : Actor, IOrderActor
    {
        public OrderActor(ActorHost host)
            : base(host)
        {
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {
            var stockActor = ProxyFactory.CreateActorProxy<IStockActor>(
                new ActorId(order.OrderNumber),
                "StockActor");
            
            var response = await stockActor.ReserveStockAsync(order.Lines);

            if (response)
            {
                order.State = OrderState.Accepted;
                await this.StateManager.SetStateAsync(
                    "order",  
                    order);  
                
                return true;
            } else {
                await CompleteOrderAsync(order, OrderState.Cancelled);  
                return false;
            }
        }

        public async Task<bool> PayOrderAsync()
        {
            // Order payment logic goes here...
            var order = this.StateManager.GetStateAsync<Order>("order").Result;
            await CompleteOrderAsync(order, OrderState.Paid);
            return true;
        }

        public Task<Order> GetOrderAsync()
        {
            return this.StateManager.GetStateAsync<Order>("order");
        }

        public Task RegisterTimer()
        {
            return this.RegisterTimerAsync(
                "OrderTimer",
                nameof(this.ReceiveTimerAsync),
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10));
        }

        public Task UnregisterTimer()
        {
            Console.WriteLine("Unregistering OrderTimer...");
            return this.UnregisterTimerAsync("OrderTimer");
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

        private async Task CompleteOrderAsync(Order order, OrderState state)
        {
            await UnregisterTimer();

            order.State = state;
            await this.StateManager.SetStateAsync(
                "order",
                order);

            // Clear ShoppingCart
            var actorType = "UserActor";
            var actorId = new ActorId(order.Username);
            var proxyUser = ActorProxy.Create<IUserActor>(actorId, actorType);
            await proxyUser.ClearShoppingCartAsync();
        }
        
        private Task ReceiveTimerAsync(byte[] state)
        {
            Console.WriteLine("ReceiveTimerAsync is called!");
            
            var order = this.StateManager.GetStateAsync<Order>("order").Result;
            CompleteOrderAsync(order, OrderState.Cancelled);
            
            // Return Stock
            var stockActor = ProxyFactory.CreateActorProxy<IStockActor>(
                new ActorId(order.OrderNumber),
                "StockActor");

            stockActor.ReturnStockAsync(order.Lines);

            return Task.CompletedTask;
        }
    }
