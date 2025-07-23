using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    [Required(ErrorMessage = "Người nhận là bắt buộc")]
    [Display(Name = "Người nhận")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Nội dung thông báo là bắt buộc")]
    [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
    [Display(Name = "Nội dung thông báo")]
    [DataType(DataType.MultilineText)]
    public string Message { get; set; } = null!;

    [Display(Name = "Ngày gửi")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? SentDate { get; set; }

    [Display(Name = "Đã đọc")]
    public bool? IsRead { get; set; }

    public virtual User User { get; set; } = null!;
}
