using Microsoft.AspNetCore.SignalR;

namespace ProjectPRN222.Hubs
{
    public class NotificationHub : Hub
    {
        // Kết nối user - đơn giản như SimpleChat
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            await base.OnConnectedAsync();
        }

        // Ngắt kết nối - đơn giản
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetHttpContext()?.Session.GetInt32("UserId");
            if (userId.HasValue && userId.Value > 0)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Gửi notification đến user - giống SendMessage trong SimpleChat
        public async Task SendNotificationToUser(int targetUserId, string message)
        {
            await Clients.Group($"User_{targetUserId}").SendAsync("ReceiveNotification", new
            {
                message = message,
                timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            });
        }
    }
} 