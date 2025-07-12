using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionAppointment
{
    public int AppointmentId { get; set; }

    [Display(Name = "Phương tiện")]
    public int VehicleId { get; set; }

    [Display(Name = "Người đặt lịch")]
    public int UserId { get; set; }

    [Display(Name = "Trạm đăng kiểm")]
    public int StationId { get; set; }

    [Required(ErrorMessage = "Ngày hẹn là bắt buộc")]
    [Display(Name = "Ngày và giờ hẹn")]
    [DataType(DataType.DateTime)]
    [FutureDate(ErrorMessage = "Ngày hẹn phải là thời gian trong tương lai")]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
    [Display(Name = "Trạng thái")]
    public string Status { get; set; } = null!;

    [Display(Name = "Ghi chú")]
    [DataType(DataType.MultilineText)]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    public string? Note { get; set; }

    public virtual InspectionStation? Station { get; set; } = null!;

    public virtual User? User { get; set; } = null!;

    public virtual Vehicle? Vehicle { get; set; } = null!;
}

// Custom validation attribute for future dates
public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }
        return true; // Allow null values, use [Required] for mandatory validation
    }
}
