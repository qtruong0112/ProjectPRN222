using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "Mã yêu cầu")]
        public string? RequestId { get; set; }

        [Display(Name = "Hiển thị mã yêu cầu")]
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
