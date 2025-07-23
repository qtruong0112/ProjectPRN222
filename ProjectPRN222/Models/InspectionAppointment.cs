using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionAppointment
{
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Phương tiện là bắt buộc")]
    [Display(Name = "Phương tiện")]
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Người đặt lịch là bắt buộc")]
    [Display(Name = "Người đặt lịch")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Trạm đăng kiểm là bắt buộc")]
    [Display(Name = "Trạm đăng kiểm")]
    public int StationId { get; set; }

    [Required(ErrorMessage = "Ngày hẹn là bắt buộc")]
    [Display(Name = "Ngày và giờ hẹn")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
    [Display(Name = "Trạng thái")]
    [RegularExpression(@"^(Pending|Confirmed|Completed|Cancelled)$", 
        ErrorMessage = "Trạng thái phải là: Pending, Confirmed, Completed, hoặc Cancelled")]
    public string Status { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    [Display(Name = "Ghi chú")]
    [DataType(DataType.MultilineText)]
    public string? Note { get; set; }

    public virtual InspectionStation Station { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
