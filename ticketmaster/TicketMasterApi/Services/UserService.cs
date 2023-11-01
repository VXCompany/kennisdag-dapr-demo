using Dapr.Actors;
using Dapr.Actors.Client;
using TicketMasterApi.Models;
using UserActor.Interfaces;

namespace TicketMasterApi.Services;

public class UserService : IUserService
{
    public async Task<bool> CreateAsync(User user)
    {
        if (user is null || string.IsNullOrWhiteSpace(user.Username))
        {
            return false;
        }
        
        // Registered Actor Type in Actor Service
        var actorType = "UserActor";

        // An ActorId uniquely identifies an actor instance
        // If the actor matching this id does not exist, it will be created
        var actorId = new ActorId(user.Username);

        // Create the local proxy by using the same interface that the service implements.
        // You need to provide the type and id so the actor can be located. 
        var proxyUser = ActorProxy.Create<IUserActor>(actorId, actorType);

        return true;
    }
}