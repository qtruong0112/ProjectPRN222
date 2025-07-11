using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    [Display(Name = "Người dùng")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Nội dung thông báo là bắt buộc")]
    [Display(Name = "Nội dung thông báo")]
    [DataType(DataType.MultilineText)]
    public string Message { get; set; } = null!;

    [Display(Name = "Ngày gửi")]
    [DataType(DataType.DateTime)]
    public DateTime? SentDate { get; set; }

    [Display(Name = "Đã đọc")]
    public bool? IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}
