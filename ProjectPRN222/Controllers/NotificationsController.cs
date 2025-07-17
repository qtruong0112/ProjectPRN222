using Microsoft.AspNetCore.Mvc;
using ProjectPRN222.Models;
using ProjectPRN222.Services;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly PrnprojectContext _context;
        private readonly NotificationService _notificationService;

        public NotificationsController(PrnprojectContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Lấy UserID từ session
        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId ?? 0;
        }

        // Trang danh sách thông báo
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            ViewBag.UnreadCount = await _notificationService.GetUnreadCountAsync(userId);
            
            return View(notifications);
        }

        // API: Lấy danh sách thông báo (JSON)
        [HttpGet]
        public async Task<IActionResult> GetNotifications(int skip = 0, int take = 20)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, skip, take);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

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

        // API: Đánh dấu thông báo đã đọc
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (result)
            {
                return Json(new { success = true, message = "Đã đánh dấu là đã đọc" });
            }

            return Json(new { success = false, message = "Không tìm thấy thông báo" });
        }

        // API: Đánh dấu tất cả thông báo đã đọc
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            await _notificationService.MarkAllAsReadAsync(userId);
            return Json(new { success = true, message = "Đã đánh dấu tất cả là đã đọc" });
        }

        // API: Xóa thông báo
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            if (result)
            {
                return Json(new { success = true, message = "Đã xóa thông báo" });
            }

            return Json(new { success = false, message = "Không tìm thấy thông báo" });
        }

        // API: Lấy số lượng thông báo chưa đọc
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, count = 0 });
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { success = true, count = count });
        }

        // API: Tạo thông báo mới (chỉ dành cho admin hoặc hệ thống)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            // Kiểm tra quyền admin hoặc hệ thống tại đây
            if (string.IsNullOrEmpty(request.Message) || request.UserId <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }

            var notification = await _notificationService.CreateNotificationAsync(
                request.UserId, 
                request.Message, 
                request.Title ?? "Thông báo"
            );

            return Json(new { success = true, data = notification });
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