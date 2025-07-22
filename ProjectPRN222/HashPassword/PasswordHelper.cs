using Microsoft.CodeAnalysis.Scripting;

namespace ProjectPRN222.HashPassword
{
    public class PasswordHelper
    {
        // Hash mật khẩu
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // So sánh mật khẩu nhập với hash đã lưu
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
