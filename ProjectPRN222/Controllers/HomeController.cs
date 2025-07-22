using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN222.Models;

namespace ProjectPRN222.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserName = HttpContext.Session.GetString("FullName");
            ViewBag.RoleId = HttpContext.Session.GetInt32("RoleId");
            ViewBag.Email = HttpContext.Session.GetString("Email");

            // Kiểm tra xem user đã đăng nhập chưa
            ViewBag.IsLoggedIn = ViewBag.UserId != null;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View("~/Views/Home/AccessDenied.cshtml");
        }
    }
}
