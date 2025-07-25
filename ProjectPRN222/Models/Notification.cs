using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Notification
{
    [Key]
    [Display(Name = "Mã thông báo")]
    public int NotificationId { get; set; }

    [Required(ErrorMessage = "Người nhận là bắt buộc")]
    [Display(Name = "Người nhận")]
    public int UserId { get; set; }

    [Display(Name = "Nội dung")]
    [StringLength(100, ErrorMessage = "Nội dung không được vượt quá 100 ký tự")]
    [DataType(DataType.MultilineText)]
    public string? Message { get; set; }

    [Display(Name = "Ngày gửi")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? SentDate { get; set; }

    [Display(Name = "Đã đọc")]
    public bool? IsRead { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
