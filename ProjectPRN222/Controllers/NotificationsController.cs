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

        private int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        // Trang danh sách thông báo
        public IActionResult Index() => GetUserId() == 0 ? RedirectToAction("Login", "Accounts") : View();

        // Trang tạo thông báo - admin only
        [RoleAllow(5)]
        public IActionResult Create() 
        {
            // Load danh sách roles để hiển thị trong dropdown
            ViewBag.Roles = _context.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();
            return View();
        }

        // Xử lý tạo thông báo - admin only
        [RoleAllow(5)]
        [HttpPost]
        public async Task<IActionResult> Create(string notificationType, int? userId, int? roleId, string message)
        {
            // Load danh sách roles lại để hiển thị trong trường hợp có lỗi
            ViewBag.Roles = _context.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();

            if (string.IsNullOrEmpty(message))
            {
                ViewBag.Error = "Vui lòng nhập nội dung thông báo";
                return View();
            }

            try
            {
                int sentCount = 0;

                switch (notificationType)
                {
                    case "single":
                        if (userId == null || userId <= 0)
                        {
                            ViewBag.Error = "Vui lòng nhập User ID hợp lệ";
                            return View();
                        }
                        bool singleResult = await CreateNotification(userId.Value, message);
                        sentCount = singleResult ? 1 : 0;
                        break;

                    case "role":
                        if (roleId == null || roleId <= 0)
                        {
                            ViewBag.Error = "Vui lòng chọn vai trò";
                            return View();
                        }
                        sentCount = await CreateNotificationForRole(roleId.Value, message);
                        break;

                    case "all":
                        sentCount = await CreateNotificationForAll(message);
                        break;

                    default:
                        ViewBag.Error = "Vui lòng chọn loại thông báo";
                        return View();
                }

                if (sentCount > 0)
                {
                    ViewBag.Success = $"Gửi thông báo thành công cho {sentCount} người dùng!";
                }
                else
                {
                    ViewBag.Error = "Không có người dùng nào nhận được thông báo";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra khi gửi thông báo: " + ex.Message;
            }

            return View();
        }

        // Tạo thông báo - core method
        public async Task<bool> CreateNotification(int userId, string message)
        {
            try
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Message = message,
                    SentDate = DateTime.Now,
                    IsRead = false
                });

                await _context.SaveChangesAsync();
                await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
                {
                    message,
                    timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                });

                return true;
            }
            catch { return false; }
        }

        // Tạo thông báo cho tất cả user có role cụ thể
        private async Task<int> CreateNotificationForRole(int roleId, string message)
        {
            try
            {
                var users = await _context.Users.Where(u => u.RoleId == roleId).ToListAsync();
                int sentCount = 0;

                foreach (var user in users)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = user.UserId,
                        Message = message,
                        SentDate = DateTime.Now,
                        IsRead = false
                    });

                    // Gửi SignalR notification
                    await _hubContext.Clients.Group($"User_{user.UserId}").SendAsync("ReceiveNotification", new
                    {
                        message,
                        timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    });

                    sentCount++;
                }

                await _context.SaveChangesAsync();
                return sentCount;
            }
            catch
            {
                return 0;
            }
        }

        // Tạo thông báo cho tất cả user
        private async Task<int> CreateNotificationForAll(string message)
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                int sentCount = 0;

                foreach (var user in users)
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = user.UserId,
                        Message = message,
                        SentDate = DateTime.Now,
                        IsRead = false
                    });

                    // Gửi SignalR notification
                    await _hubContext.Clients.Group($"User_{user.UserId}").SendAsync("ReceiveNotification", new
                    {
                        message,
                        timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    });

                    sentCount++;
                }

                await _context.SaveChangesAsync();
                return sentCount;
            }
            catch
            {
                return 0;
            }
        }

        // API: Lấy danh sách thông báo
        [HttpGet]
        public async Task<IActionResult> GetNotifications(int take = 20)
        {
            var userId = GetUserId();
            if (userId == 0) return Json(new { success = false });

            var data = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentDate)
                .Take(take)
                .Select(n => new { n.NotificationId, n.Message, n.SentDate, n.IsRead })
                .ToListAsync();

            return Json(new
            {
                success = true,
                data = data.Select(n => new
                {
                    id = n.NotificationId,
                    message = n.Message,
                    sentDate = n.SentDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    isRead = n.IsRead ?? false
                })
            });
        }

        // API: Đánh dấu đã đọc
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == GetUserId());

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Json(new { success = notification != null });
        }

        // API: Đánh dấu tất cả đã đọc
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var count = await _context.Notifications
                .Where(n => n.UserId == GetUserId() && (n.IsRead == null || n.IsRead == false))
                .ExecuteUpdateAsync(n => n.SetProperty(p => p.IsRead, true));

            return Json(new { success = true, count });
        }

        // API: Xóa thông báo
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _context.Notifications
                .Where(n => n.NotificationId == id && n.UserId == GetUserId())
                .ExecuteDeleteAsync();

            return Json(new { success = count > 0 });
        }

        // API: Số lượng chưa đọc
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            if (userId == 0) return Json(new { success = false, count = 0 });

            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && (n.IsRead == null || n.IsRead == false));

            return Json(new { success = true, count });
        }
    }
} 