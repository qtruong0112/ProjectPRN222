using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Log
{
    [Key]
    [Display(Name = "Mã nhật ký")]
    public int LogId { get; set; }

    [Required(ErrorMessage = "Người dùng là bắt buộc")]
    [Display(Name = "Người dùng")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Hành động là bắt buộc")]
    [Display(Name = "Hành động")]
    [StringLength(100, ErrorMessage = "Hành động không được vượt quá 100 ký tự")]
    public string Action { get; set; } = null!;

    [Display(Name = "Thời gian")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? Timestamp { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
