using Microsoft.AspNetCore.Mvc;
using ProjectPRN222.Models;
using ProjectPRN222.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectPRN222.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly PrnprojectContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationsController(PrnprojectContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Lấy UserID từ session
        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId ?? 0;
        }

        // Tạo thông báo mới
        private async Task<Notification> CreateNotificationAsync(int userId, string message, string title = "Thông báo")
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
        private async Task<bool> MarkAsReadAsync(int notificationId, int userId)
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
        private async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
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
        private async Task<List<Notification>> GetUserNotificationsAsync(int userId, int skip = 0, int take = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        // Đếm số thông báo chưa đọc
        private async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && (n.IsRead == null || n.IsRead == false));
        }

        // Đánh dấu tất cả thông báo đã đọc
        private async Task MarkAllAsReadAsync(int userId)
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

        // Trang danh sách thông báo - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var notifications = await GetUserNotificationsAsync(userId);
            ViewBag.UnreadCount = await GetUnreadCountAsync(userId);
            
            return View(notifications);
        }

        // API: Lấy danh sách thông báo (JSON) - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        [HttpGet]
        public async Task<IActionResult> GetNotifications(int skip = 0, int take = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var notifications = await GetUserNotificationsAsync(userId, skip, take);
            var unreadCount = await GetUnreadCountAsync(userId);

            return Json(new
            {
                success = true,
                data = notifications.Select(n => new
                {
                    id = n.NotificationId,
                    message = n.Message,
                    sentDate = n.SentDate?.ToString("dd/MM/yyyy HH:mm"),
                    isRead = n.IsRead ?? false
                }),
                unreadCount = unreadCount
            });
        }

        // API: Đánh dấu thông báo đã đọc - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await MarkAsReadAsync(id, userId);
            if (result)
            {
                return Json(new { success = true, message = "Đã đánh dấu là đã đọc" });
            }

            return Json(new { success = false, message = "Không tìm thấy thông báo" });
        }

        // API: Đánh dấu tất cả thông báo đã đọc - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            await MarkAllAsReadAsync(userId);
            return Json(new { success = true, message = "Đã đánh dấu tất cả là đã đọc" });
        }

        // API: Xóa thông báo - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await DeleteNotificationAsync(id, userId);
            if (result)
            {
                return Json(new { success = true, message = "Đã xóa thông báo" });
            }

            return Json(new { success = false, message = "Không tìm thấy thông báo" });
        }

        // API: Lấy số lượng thông báo chưa đọc - Tất cả user đã đăng nhập
        [RoleAllow(1, 2, 3, 4, 5)]
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, count = 0 });
            }

            var count = await GetUnreadCountAsync(userId);
            return Json(new { success = true, count = count });
        }

        // API: Tạo thông báo mới - Chỉ Admin
        [RoleAllow(5)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            if (string.IsNullOrEmpty(request.Message) || request.UserId <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var notification = await CreateNotificationAsync(
                request.UserId, 
                request.Message, 
                request.Title ?? "Thông báo"
            );

            return Json(new { success = true, data = notification });
        }

        // Form tạo thông báo (GET) - Chỉ Admin
        [RoleAllow(5)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // API: Kiểm tra và gửi thông báo nhắc nhở kiểm định - Chỉ Admin
        [RoleAllow(5)]
        [HttpPost]
        public async Task<IActionResult> CheckInspectionReminders()
        {
            var processedCount = 0;
            var errorCount = 0;

            try
            {
                // Lấy tất cả xe cần kiểm tra
                var vehicles = await _context.Vehicles
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .ToListAsync();

                foreach (var vehicle in vehicles)
                {
                    try
                    {
                        // Tìm bản ghi kiểm định gần nhất
                        var latestRecord = vehicle.InspectionRecords
                            .OrderByDescending(r => r.InspectionDate)
                            .FirstOrDefault();

                        DateTime? nextInspectionDue = null;

                        if (latestRecord == null)
                        {
                            // Xe chưa từng kiểm định - cần kiểm định ngay
                            nextInspectionDue = DateTime.Now.AddDays(7); // Cho 7 ngày grace period
                        }
                        else if (latestRecord.Result == "Pass" && latestRecord.InspectionDate.HasValue)
                        {
                            // Xe đã pass - hạn kiểm định sau 6 tháng
                            nextInspectionDue = latestRecord.InspectionDate.Value.AddMonths(6);
                        }
                        else if (latestRecord.Result == "Fail" && latestRecord.InspectionDate.HasValue)
                        {
                            // Xe fail - cần kiểm định lại trong 1 tháng
                            nextInspectionDue = latestRecord.InspectionDate.Value.AddMonths(1);
                        }

                        if (nextInspectionDue.HasValue)
                        {
                            var daysUntilDue = (nextInspectionDue.Value - DateTime.Now).Days;
                            
                            // Gửi thông báo nhắc nhở khi còn 30 ngày, 15 ngày, 7 ngày và 1 ngày
                            if (daysUntilDue == 30 || daysUntilDue == 15 || daysUntilDue == 7 || daysUntilDue == 1)
                            {
                                var vehicleInfo = $"{vehicle.PlateNumber} ({vehicle.Brand} {vehicle.Model})";
                                await SendInspectionReminderNotificationAsync(
                                    vehicle.OwnerId, 
                                    vehicleInfo, 
                                    nextInspectionDue.Value
                                );
                                processedCount++;
                            }
                            
                            // Gửi thông báo quá hạn
                            else if (daysUntilDue < 0)
                            {
                                var vehicleInfo = $"{vehicle.PlateNumber} ({vehicle.Brand} {vehicle.Model})";
                                var overdueDays = Math.Abs(daysUntilDue);
                                
                                await CreateNotificationAsync(
                                    vehicle.OwnerId,
                                    $"Xe {vehicleInfo} đã quá hạn kiểm định {overdueDays} ngày. Vui lòng kiểm định ngay lập tức!",
                                    "Cảnh báo quá hạn kiểm định"
                                );
                                processedCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        // Log error but continue processing other vehicles
                        Console.WriteLine($"Error processing vehicle {vehicle.PlateNumber}: {ex.Message}");
                    }
                }

                return Json(new 
                { 
                    success = true, 
                    message = $"Đã xử lý {processedCount} thông báo nhắc nhở. Có {errorCount} lỗi.",
                    processed = processedCount,
                    errors = errorCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
    }

    // Model cho request tạo thông báo
    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Title { get; set; }
    }
} 