using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Vehicle
{
    [Key]
    [Display(Name = "Mã phương tiện")]
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Chủ sở hữu là bắt buộc")]
    [Display(Name = "Chủ sở hữu")]
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "Biển số xe là bắt buộc")]
    [Display(Name = "Biển số xe")]
    [StringLength(15, ErrorMessage = "Biển số xe không được vượt quá 15 ký tự")]
    [RegularExpression(@"^[0-9]{2}[A-Z]{1}-[0-9]{4,5}$|^[0-9]{2}[A-Z]{2}-[0-9]{4,5}$", ErrorMessage = "Định dạng biển số xe không hợp lệ (VD: 29A-12345)")]
    public string PlateNumber { get; set; } = null!;

    [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
    [Display(Name = "Thương hiệu")]
    [StringLength(50, ErrorMessage = "Thương hiệu không được vượt quá 50 ký tự")]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Model là bắt buộc")]
    [Display(Name = "Model")]
    [StringLength(50, ErrorMessage = "Model không được vượt quá 50 ký tự")]
    public string Model { get; set; } = null!;

    [Required(ErrorMessage = "Năm sản xuất là bắt buộc")]
    [Display(Name = "Năm sản xuất")]
    [Range(1950, 2030, ErrorMessage = "Năm sản xuất phải từ 1950 đến 2030")]
    public int ManufactureYear { get; set; }

    [Required(ErrorMessage = "Số máy là bắt buộc")]
    [Display(Name = "Số máy")]
    [StringLength(100, ErrorMessage = "Số máy không được vượt quá 100 ký tự")]
    [RegularExpression(@"^[A-Z0-9]{6,20}$", ErrorMessage = "Số máy chỉ được gồm chữ in hoa và số, từ 6 đến 20 ký tự, không có dấu cách")]
    public string EngineNumber { get; set; } = null!;


    // Navigation properties
    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User? Owner { get; set; } = null!;
}
