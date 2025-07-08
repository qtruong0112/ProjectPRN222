using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Log
{
    [Display(Name = "Mã nhật ký")]
    public int LogId { get; set; }

    [Display(Name = "Mã người dùng")]
    [Required(ErrorMessage = "Vui lòng chọn người dùng")]
    public int UserId { get; set; }

    [Display(Name = "Hành động")]
    [Required(ErrorMessage = "Vui lòng nhập hành động")]
    [StringLength(100, ErrorMessage = "Hành động không được vượt quá 100 ký tự")]
    public string Action { get; set; } = null!;

    [Display(Name = "Thời gian")]
    [DataType(DataType.DateTime)]
    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
