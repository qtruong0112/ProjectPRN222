using System.Net.Mail;
using System.Net;

namespace ProjectPRN222.Services
{
    public static class EmailService
    {
        public static async Task SendResetPasswordEmail(string toEmail, string resetLink)
        {
            var fromEmail = "pqtruong011203@gmail.com";
            var password = "xren wads japo tvvf"; 

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.To.Add(toEmail);
            message.Subject = "Đặt lại mật khẩu";
            message.Body = $"Nhấp vào liên kết sau để đặt lại mật khẩu của bạn:\n\n{resetLink}";
            message.IsBodyHtml = false;

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(fromEmail, password);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }
    }
}
