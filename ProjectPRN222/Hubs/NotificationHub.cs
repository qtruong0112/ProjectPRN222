using Microsoft.AspNetCore.SignalR;
using ProjectPRN222.Models;

namespace ProjectPRN222.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly PrnprojectContext _context;

        public NotificationHub(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public override async Task OnConnectedAsync()
        {
            // Tự động join user vào group khi kết nối
            var userId = Context.GetHttpContext()?.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetHttpContext()?.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
} 