using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Log
{
    public int LogId { get; set; }

    [Display(Name = "Người dùng")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Hành động là bắt buộc")]
    [StringLength(100, ErrorMessage = "Mô tả hành động không được vượt quá 100 ký tự")]
    [Display(Name = "Hành động")]
    public string Action { get; set; } = null!;

    [Display(Name = "Thời gian")]
    [DataType(DataType.DateTime)]
    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
