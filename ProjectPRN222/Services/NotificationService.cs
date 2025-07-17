using Microsoft.AspNetCore.SignalR;
using ProjectPRN222.Hubs;
using ProjectPRN222.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Services
{
    public class NotificationService
    {
        private readonly PrnprojectContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(PrnprojectContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Tạo thông báo mới
        public async Task<Notification> CreateNotificationAsync(int userId, string message, string title = "Thông báo")
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Gửi thông báo realtime qua SignalR
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
            {
                id = notification.NotificationId,
                message = notification.Message,
                sentDate = notification.SentDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                isRead = notification.IsRead,
                title = title
            });

            return notification;
        }

        // Gửi thông báo khi có kết quả kiểm định mới
        public async Task SendInspectionResultNotificationAsync(int vehicleOwnerId, string vehicleInfo, string result)
        {
            var message = $"Kết quả kiểm định cho xe {vehicleInfo}: {result}";
            await CreateNotificationAsync(vehicleOwnerId, message, "Kết quả kiểm định");
        }

        // Gửi thông báo nhắc nhở hạn kiểm định
        public async Task SendInspectionReminderNotificationAsync(int userId, string vehicleInfo, DateTime inspectionDue)
        {
            var message = $"Xe {vehicleInfo} cần kiểm định trước ngày {inspectionDue:dd/MM/yyyy}";
            await CreateNotificationAsync(userId, message, "Nhắc nhở kiểm định");
        }

        // Đánh dấu thông báo đã đọc
        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Xóa thông báo
        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Lấy danh sách thông báo của user
        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int skip = 0, int take = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        // Đếm số thông báo chưa đọc
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && (n.IsRead == null || n.IsRead == false));
        }

        // Đánh dấu tất cả thông báo đã đọc
        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && (n.IsRead == null || n.IsRead == false))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
} 