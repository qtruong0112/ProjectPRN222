using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
