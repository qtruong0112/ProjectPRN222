    using Microsoft.AspNetCore.Mvc;
    using ProjectPRN222.Models;
    using ProjectPRN222.HashPassword;
    using Microsoft.AspNetCore.Http;
    using Humanizer;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
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
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.RoleId);
                return View(user);
            }

            // POST: Users/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Email,Password,Phone,Address,RoleId")] User user)
            {
                if (id != user.UserId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
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
                return View(user);
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


