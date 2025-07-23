using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [Display(Name = "Số điện thoại")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
    public string Phone { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    [Display(Name = "Địa chỉ")]
    [DataType(DataType.MultilineText)]
    public string? Address { get; set; }

    [Display(Name = "Vai trò")]
    public int? RoleId { get; set; }

    [StringLength(255, ErrorMessage = "Token không được vượt quá 255 ký tự")]
    [Display(Name = "Token đặt lại mật khẩu")]
    public string? ResetPasswordToken { get; set; }

    [Display(Name = "Thời gian hết hạn token")]
    [DataType(DataType.DateTime)]
    public DateTime? TokenExpiry { get; set; }

    public int? StationId { get; set; }

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role? Role { get; set; }

    public virtual InspectionStation? Station { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
