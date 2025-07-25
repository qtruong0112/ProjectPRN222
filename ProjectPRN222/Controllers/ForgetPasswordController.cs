using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;
using ProjectPRN222.HashPassword;
using ProjectPRN222.Services;

namespace ProjectPRN222.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly PrnprojectContext _context;

        public ForgotPasswordController(PrnprojectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Vui lòng nhập địa chỉ email.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email không tồn tại.");
                return View();
            }

            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var resetLink = Url.Action("ResetPassword", "ForgotPassword", new { token = token }, Request.Scheme);
            
            try
            {
                await EmailService.SendResetPasswordEmail(user.Email, resetLink);
                ViewBag.Message = "Email đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra hộp thư của bạn.";
            }
            catch
            {
                ViewBag.Error = "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return NotFound("Token không hợp lệ.");
            }

            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token && u.TokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                return NotFound("Token không hợp lệ hoặc đã hết hạn.");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == model.Token && u.TokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                ModelState.AddModelError("", "Token không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            user.Password = PasswordHelper.HashPassword(model.NewPassword);
            user.ResetPasswordToken = null;
            user.TokenExpiry = null;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Mật khẩu đã được đặt lại thành công.";
            return RedirectToAction("Login", "Accounts");
        }
    }
}