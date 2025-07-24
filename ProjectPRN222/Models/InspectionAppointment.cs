using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionAppointment
{
    [Key]
    [Display(Name = "Mã lịch hẹn")]
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Phương tiện là bắt buộc")]
    [Display(Name = "Phương tiện")]
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Người đặt lịch là bắt buộc")]
    [Display(Name = "Người đặt lịch")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Trạm kiểm định là bắt buộc")]
    [Display(Name = "Trạm kiểm định")]
    public int StationId { get; set; }

    [Required(ErrorMessage = "Ngày hẹn là bắt buộc")]
    [Display(Name = "Ngày hẹn")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc")]
    [Display(Name = "Trạng thái")]
    [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
    public string Status { get; set; } = null!;

    [Display(Name = "Ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
    [DataType(DataType.MultilineText)]
    public string? Note { get; set; }

    // Navigation properties
    public virtual InspectionStation Station { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
