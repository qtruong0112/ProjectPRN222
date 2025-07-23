using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionStation
{
    public int StationId { get; set; }

    [Required(ErrorMessage = "Tên trạm đăng kiểm là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên trạm không được vượt quá 100 ký tự")]
    [Display(Name = "Tên trạm đăng kiểm")]
    public string? Name { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
    [Display(Name = "Địa chỉ")]
    [DataType(DataType.MultilineText)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [Display(Name = "Số điện thoại")]
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();
}
