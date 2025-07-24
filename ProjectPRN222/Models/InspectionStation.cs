using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionStation
{
    [Key]
    [Display(Name = "Mã trạm")]
    public int StationId { get; set; }

    [Required(ErrorMessage = "Tên trạm là bắt buộc")]
    [Display(Name = "Tên trạm")]
    [StringLength(100, ErrorMessage = "Tên trạm không được vượt quá 100 ký tự")]
    public string? Name { get; set; }

    [Display(Name = "Địa chỉ")]
    [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    [DataType(DataType.MultilineText)]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
    [RegularExpression("^\\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    public string Email { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
