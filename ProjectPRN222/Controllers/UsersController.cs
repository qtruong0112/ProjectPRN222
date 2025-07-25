using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;

namespace ProjectPRN222.Controllers
{
    [RoleAllow(5, 3)] // Admin và InspectionCenter
    public class UsersController : Controller
    {
        private readonly PrnprojectContext _context;

        public UsersController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users.FindAsync(currentUserId);

            var query = _context.Users.Include(u => u.Role).AsQueryable();

            // Nếu user là InspectionCenter (role 3), chỉ hiển thị Workers (role 2) của trạm mình
            if (currentUser != null && currentUser.RoleId == 3 && currentUser.StationId.HasValue)
            {
                query = query.Where(u => u.RoleId == 2 && u.StationId == currentUser.StationId.Value);
            }

            return View(await query.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho InspectionCenter
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users.FindAsync(currentUserId);
            
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể xem Workers của trạm mình
                if (user.RoleId != 2 || user.StationId != currentUser.StationId)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }

            return View(user);
        }

        // GET: Users/Create
        
        public async Task<IActionResult> Create()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            // Nếu là InspectionCenter, chỉ cho phép tạo Workers cho trạm mình
            if (currentUser != null && currentUser.RoleId == 3)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName");
                ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name");
                ViewBag.CurrentUserRole = 3;
                ViewBag.CurrentUserStationId = currentUser.StationId;
                ViewBag.CurrentUserStationName = currentUser.Station?.Name;
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
                ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name");
                ViewBag.CurrentUserRole = currentUser?.RoleId;
            }
            
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Email,Password,Phone,Address,RoleId,StationId")] User user)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            // Kiểm tra quyền tạo user cho InspectionCenter
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể tạo Workers cho trạm mình
                if (user.RoleId != 2 || user.StationId != currentUser.StationId)
                {
                    ModelState.AddModelError("", "Bạn chỉ có thể tạo nhân viên cho trạm của mình.");
                    ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                    ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                    ViewBag.CurrentUserRole = 3;
                    ViewBag.CurrentUserStationId = currentUser.StationId;
                    ViewBag.CurrentUserStationName = currentUser.Station?.Name;
                    return View(user);
                }
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng email
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existing != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    if (currentUser != null && currentUser.RoleId == 3)
                    {
                        ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                        ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                        ViewBag.CurrentUserRole = 3;
                        ViewBag.CurrentUserStationId = currentUser.StationId;
                        ViewBag.CurrentUserStationName = currentUser.Station?.Name;
                    }
                    else
                    {
                        ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                        ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
                        ViewBag.CurrentUserRole = currentUser?.RoleId;
                    }
                    return View(user);
                }
                // Kiểm tra trùng số điện thoại
                var existingPhone = await _context.Users.FirstOrDefaultAsync(u => u.Phone == user.Phone);
                if (existingPhone != null)
                {
                    ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng.");
                    if (currentUser != null && currentUser.RoleId == 3)
                    {
                        ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                        ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                        ViewBag.CurrentUserRole = 3;
                        ViewBag.CurrentUserStationId = currentUser.StationId;
                        ViewBag.CurrentUserStationName = currentUser.Station?.Name;
                    }
                    else
                    {
                        ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                        ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
                        ViewBag.CurrentUserRole = currentUser?.RoleId;
                    }
                    return View(user);
                }

                // Nếu role không phải Worker (2) hoặc InspectionCenter (3) thì set StationId = null
                if (user.RoleId != 2 && user.RoleId != 3)
                {
                    user.StationId = null;
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Truyền lại ViewBag khi ModelState không hợp lệ
            if (currentUser != null && currentUser.RoleId == 3)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = 3;
                ViewBag.CurrentUserStationId = currentUser.StationId;
                ViewBag.CurrentUserStationName = currentUser.Station?.Name;
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = currentUser?.RoleId;
            }
            return View(user);
        }
        [RoleAllow(5, 1, 3)] // Admin, Owner, InspectionCenter
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho InspectionCenter
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);
            
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể edit Workers của trạm mình
                if (user.RoleId != 2 || user.StationId != currentUser.StationId)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
                
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = 3;
                ViewBag.CurrentUserStationId = currentUser.StationId;
                ViewBag.CurrentUserStationName = currentUser.Station?.Name;
                ViewBag.EditingUserStationName = user.Station?.Name;
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = currentUser?.RoleId;
                ViewBag.EditingUserStationName = user.Station?.Name;
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAllow(5, 1, 3)] // Admin, Owner, InspectionCenter
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Email,Phone,Address,RoleId,StationId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho InspectionCenter
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);
            
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể edit Workers của trạm mình
                var originalUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                if (originalUser == null || originalUser.RoleId != 2 || originalUser.StationId != currentUser.StationId)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
                
                // InspectionCenter không thể thay đổi role và station
                user.RoleId = originalUser.RoleId;
                user.StationId = originalUser.StationId;
            }

            // Xóa lỗi validation cho Password vì chúng ta không edit nó
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy user hiện tại từ database
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật chỉ những trường được phép edit, giữ nguyên Password và các trường khác
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Address = user.Address;
                    existingUser.RoleId = user.RoleId;
                    
                    // Cập nhật StationId, nếu role không phải Worker (2) hoặc InspectionCenter (3) thì set null
                    if (user.RoleId == 2 || user.RoleId == 3) // Worker role hoặc InspectionCenter role
                    {
                        existingUser.StationId = user.StationId;
                    }
                    else
                    {
                        existingUser.StationId = null;
                    }
                    
                    // Password, ResetPasswordToken, TokenExpiry giữ nguyên

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            // Truyền lại ViewBag khi ModelState không hợp lệ
            var editingUser = await _context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.UserId == id);
                
            if (currentUser != null && currentUser.RoleId == 3)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles.Where(r => r.RoleId == 2), "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations.Where(s => s.StationId == currentUser.StationId), "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = 3;
                ViewBag.CurrentUserStationId = currentUser.StationId;
                ViewBag.CurrentUserStationName = currentUser.Station?.Name;
                ViewBag.EditingUserStationName = editingUser?.Station?.Name;
            }
            else
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
                ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
                ViewBag.CurrentUserRole = currentUser?.RoleId;
                ViewBag.EditingUserStationName = editingUser?.Station?.Name;
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [RoleAllow(5, 1, 3)] // Admin, Owner, InspectionCenter
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Station)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho InspectionCenter
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users.FindAsync(currentUserId);
            
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể xóa Workers của trạm mình
                if (user.RoleId != 2 || user.StationId != currentUser.StationId)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAllow(5, 1, 3)] // Admin, Owner, InspectionCenter
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users
                .Include(u => u.Notifications)
                .Include(u => u.InspectionRecords)
                .Include(u => u.Vehicles)
                .Include(u => u.InspectionAppointments)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập cho InspectionCenter
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users.FindAsync(currentUserId);
            
            if (currentUser != null && currentUser.RoleId == 3)
            {
                // InspectionCenter chỉ có thể xóa Workers của trạm mình
                if (user.RoleId != 2 || user.StationId != currentUser.StationId)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }

            if (user.Notifications.Any() || user.InspectionRecords.Any() || user.Vehicles.Any() || user.InspectionAppointments.Any())
            {
                TempData["DeleteError"] = "Không thể xóa người dùng vì còn dữ liệu liên quan (thông báo, xe, lịch hẹn, kiểm định ...).";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
