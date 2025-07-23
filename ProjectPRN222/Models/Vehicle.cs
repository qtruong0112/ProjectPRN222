using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Chủ sở hữu là bắt buộc")]
    [Display(Name = "Chủ sở hữu")]
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "Biển số xe là bắt buộc")]
    [StringLength(15, ErrorMessage = "Biển số xe không được vượt quá 15 ký tự")]
    [Display(Name = "Biển số xe")]
    [RegularExpression(@"^[0-9]{2}[A-Z]{1,2}-[0-9]{3,5}$", ErrorMessage = "Biển số xe không đúng định dạng (VD: 30A-12345)")]
    public string PlateNumber { get; set; } = null!;

    [Required(ErrorMessage = "Hãng xe là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên hãng xe không được vượt quá 50 ký tự")]
    [Display(Name = "Hãng xe")]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Mẫu xe là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên mẫu xe không được vượt quá 50 ký tự")]
    [Display(Name = "Mẫu xe")]
    public string Model { get; set; } = null!;

    [Required(ErrorMessage = "Năm sản xuất là bắt buộc")]
    [Range(1950, 2025, ErrorMessage = "Năm sản xuất phải từ 1950 đến 2025")]
    [Display(Name = "Năm sản xuất")]
    public int ManufactureYear { get; set; }

    [Required(ErrorMessage = "Số máy là bắt buộc")]
    [StringLength(100, ErrorMessage = "Số máy không được vượt quá 100 ký tự")]
    [Display(Name = "Số máy")]
    public string EngineNumber { get; set; } = null!;

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User? Owner { get; set; } = null!;
}
