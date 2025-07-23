using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Log
{
    public int LogId { get; set; }

    [Required(ErrorMessage = "Người dùng là bắt buộc")]
    [Display(Name = "Người dùng")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Hành động là bắt buộc")]
    [StringLength(100, ErrorMessage = "Hành động không được vượt quá 100 ký tự")]
    [Display(Name = "Hành động")]
    public string Action { get; set; } = null!;

    [Display(Name = "Thời gian")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
