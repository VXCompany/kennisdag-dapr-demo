using TicketMasterApi.Models;

namespace TicketMasterApi.Services;

public interface IUserService
{
    public Task<bool> CreateAsync(User user);
}