    using Microsoft.AspNetCore.Mvc;
    using ProjectPRN222.Models;
    using ProjectPRN222.HashPassword;
    using Microsoft.AspNetCore.Http;
    using Humanizer;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ProjectPRN222.Controllers
{
        public class AccountsController : Controller
        {
            private readonly PrnprojectContext _context;

            public AccountsController(PrnprojectContext context)
            {
                _context = context;
            }
            public ActionResult Register()
            {
                return View();
            }

            [HttpPost]
            public ActionResult Register(User user)
            {
                if (ModelState.IsValid)
                {
                    var existing = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    if (existing != null)
                    {
                        ModelState.AddModelError("", "Email đã được sử dụng.");
                        return View(user);
                    }

                    // Kiểm tra trùng số điện thoại
                    var existingPhone = _context.Users.FirstOrDefault(u => u.Phone == user.Phone);
                    if (existingPhone != null)
                    {
                        ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng.");
                        return View(user);
                    }

                    user.Password = PasswordHelper.HashPassword(user.Password);
                    user.RoleId = 1;
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }
                return View(user);
            }

            // GET: /Account/Login
            public ActionResult Login()
            {
                return View();
            }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                bool isPasswordValid;

                // Kiểm tra xem mật khẩu trong DB có phải là hash hay không (đơn giản là kiểm tra bắt đầu bằng "$2")
                if (user.Password.StartsWith("$2")) // dấu hiệu của mật khẩu được hash bằng BCrypt
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                }
                else
                {
                    isPasswordValid = (password == user.Password);
                }

                if (isPasswordValid)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("FullName", user.FullName);
                    HttpContext.Session.SetInt32("RoleId", user.Role.RoleId);

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng.";
            return View();
        }


        public ActionResult Logout()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy tên trạm nếu có
            if (user.StationId.HasValue)
            {
                var station = await _context.InspectionStations
                    .FirstOrDefaultAsync(s => s.StationId == user.StationId.Value);
                ViewBag.StationName = station?.Name ?? "Không xác định";
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            ViewBag.StationList = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
            return View(user);
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Email,Phone,StationId,Address,RoleId,Password")] User user)
            {
                if (id != user.UserId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Get existing user to preserve password
                        var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                        if (existingUser == null)
                        {
                            return NotFound();
                        }

                        // Preserve the original password
                        user.Password = existingUser.Password;
                        
                        _context.Update(user);
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
                    return RedirectToAction("Index", "Home");
                }
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
            ViewBag.StationList = new SelectList(_context.InspectionStations, "StationId", "Name", user.StationId);
            return View(user);
            }

            // GET: ChangePassword
            public IActionResult ChangePassword()
            {
                return View();
            }

            // POST: ChangePassword
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login");
                }

                // Validation
                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại là bắt buộc");
                }
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ModelState.AddModelError("newPassword", "Mật khẩu mới là bắt buộc");
                }
                else if (newPassword.Length < 6)
                {
                    ModelState.AddModelError("newPassword", "Mật khẩu mới phải có ít nhất 6 ký tự");
                }
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Xác nhận mật khẩu không khớp");
                }

                if (!ModelState.IsValid)
                {
                    return View();
                }

                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Verify current password
                    bool isCurrentPasswordValid;
                    if (user.Password.StartsWith("$2")) // BCrypt hash
                    {
                        isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.Password);
                    }
                    else
                    {
                        isCurrentPasswordValid = (currentPassword == user.Password);
                    }

                    if (!isCurrentPasswordValid)
                    {
                        ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không đúng");
                        return View();
                    }

                    // Update password with hash
                    user.Password = PasswordHelper.HashPassword(newPassword);
                    await _context.SaveChangesAsync();

                    ViewBag.Success = "Đổi mật khẩu thành công!";
                    return View();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi đổi mật khẩu. Vui lòng thử lại.");
                    return View();
                }
            }
            private bool UserExists(int id)
            {
                return _context.Users.Any(e => e.UserId == id);
            }
        }
    }

    //Lấy session trong View hoặc Controller khác
    //int? userId = HttpContext.Session.GetInt32("UserId");
    //string fullName = HttpContext.Session.GetString("FullName");


    //@inject Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor

    //<p>Xin chào, @HttpContextAccessor.HttpContext.Session.GetString("FullName")</ p >


