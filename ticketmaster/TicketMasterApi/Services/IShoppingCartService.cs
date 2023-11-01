using TicketMasterApi.Models;

namespace TicketMasterApi.Services;

public interface IShoppingCartService
{
    public Task<bool> CreateAsync(Cart cart, string username);
    
    public Task<bool> CheckoutAsync(string username);

    public Task<bool> PayOrderAsync(string username);
}