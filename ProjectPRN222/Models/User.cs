using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class User
{
    [Key]
    [Display(Name = "Mã người dùng")]
    public int UserId { get; set; }

    [Display(Name = "Họ và tên")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [Display(Name = "Email")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự và không quá 255 ký tự")]
    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    [RegularExpression("^\\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Địa chỉ")]
    [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    public string? Address { get; set; }

    [Display(Name = "Vai trò")]
    public int? RoleId { get; set; }

    [Display(Name = "Token đặt lại mật khẩu")]
    [StringLength(255)]
    public string? ResetPasswordToken { get; set; }

    [Display(Name = "Thời gian hết hạn token")]
    [DataType(DataType.DateTime)]
    public DateTime? TokenExpiry { get; set; }

    [Display(Name = "Trạm đăng kiểm")]
    public int? StationId { get; set; }

    // Navigation properties
    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role? Role { get; set; }

    public virtual InspectionStation? Station { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
