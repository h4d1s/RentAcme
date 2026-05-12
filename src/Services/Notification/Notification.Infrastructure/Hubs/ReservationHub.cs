using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Notification.Infrastructure.Hubs;

public class ReservationHub : Hub<IReservationClient>
{
    public async Task<string> JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        return $"Successfully joined group: user:{userId}";
    }
}