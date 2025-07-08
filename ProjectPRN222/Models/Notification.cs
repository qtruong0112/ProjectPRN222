using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Notification
{
    [Display(Name = "Mã thông báo")]
    public int NotificationId { get; set; }

    [Display(Name = "Mã người dùng")]
    [Required(ErrorMessage = "Vui lòng chọn người dùng")]
    public int UserId { get; set; }

    [Display(Name = "Nội dung thông báo")]
    [Required(ErrorMessage = "Vui lòng nhập nội dung thông báo")]
    [StringLength(1000, ErrorMessage = "Nội dung thông báo không được vượt quá 1000 ký tự")]
    public string Message { get; set; } = null!;

    [Display(Name = "Ngày gửi")]
    [DataType(DataType.DateTime)]
    public DateTime? SentDate { get; set; }

    [Display(Name = "Đã đọc")]
    public bool? IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}
