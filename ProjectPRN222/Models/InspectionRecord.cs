using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionRecord
{
    [Key]
    [Display(Name = "Mã hồ sơ kiểm định")]
    public int RecordId { get; set; }

    [Required(ErrorMessage = "Phương tiện là bắt buộc")]
    [Display(Name = "Phương tiện")]
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Trạm kiểm định là bắt buộc")]
    [Display(Name = "Trạm kiểm định")]
    public int StationId { get; set; }

    [Required(ErrorMessage = "Thanh tra viên là bắt buộc")]
    [Display(Name = "Thanh tra viên")]
    public int InspectorId { get; set; }

    [Display(Name = "Ngày kiểm định")]
    [DataType(DataType.DateTime)]
    public DateTime? InspectionDate { get; set; }

    [Required(ErrorMessage = "Kết quả kiểm định là bắt buộc")]
    [Display(Name = "Kết quả")]
    [StringLength(10, ErrorMessage = "Kết quả không được vượt quá 10 ký tự")]
    public string Result { get; set; } = null!;

    [Required(ErrorMessage = "Nồng độ CO2 là bắt buộc")]
    [Display(Name = "Nồng độ CO2 (%)")]
    [Range(0.00, 99.99, ErrorMessage = "Nồng độ CO2 phải từ 0.00 đến 99.99")]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Co2emission { get; set; }

    [Required(ErrorMessage = "Nồng độ HC là bắt buộc")]
    [Display(Name = "Nồng độ HC (ppm)")]
    [Range(0.00, 99.99, ErrorMessage = "Nồng độ HC phải từ 0.00 đến 99.99")]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Hcemission { get; set; }

    [Display(Name = "Ghi chú")]
    [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
    [DataType(DataType.MultilineText)]
    public string? Comments { get; set; }

    // Navigation properties
    public virtual User? Inspector { get; set; } = null!;

    public virtual InspectionStation? Station { get; set; } = null!;

    public virtual Vehicle? Vehicle { get; set; } = null!;
}
