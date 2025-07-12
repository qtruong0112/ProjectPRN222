using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionRecord
{
    public int RecordId { get; set; }

    [Display(Name = "Phương tiện")]
    public int VehicleId { get; set; }

    [Display(Name = "Trạm đăng kiểm")]
    public int StationId { get; set; }

    [Display(Name = "Người kiểm định")]
    public int InspectorId { get; set; }

    [Display(Name = "Ngày đăng kiểm")]
    [DataType(DataType.DateTime)]
    public DateTime? InspectionDate { get; set; }

    [Required(ErrorMessage = "Kết quả đăng kiểm là bắt buộc")]
    [StringLength(10, ErrorMessage = "Kết quả không được vượt quá 10 ký tự")]
    [Display(Name = "Kết quả")]
    public string Result { get; set; } = null!;

    [Required(ErrorMessage = "Chỉ số CO2 là bắt buộc")]
    [Range(0, 99.99, ErrorMessage = "Chỉ số CO2 phải từ 0 đến 99.99")]
    [Display(Name = "Nồng độ CO2 (%)")]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Co2emission { get; set; }

    [Required(ErrorMessage = "Chỉ số HC là bắt buộc")]
    [Range(0, 99.99, ErrorMessage = "Chỉ số HC phải từ 0 đến 99.99")]
    [Display(Name = "Nồng độ HC (ppm)")]
    [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
    public decimal Hcemission { get; set; }

    [Display(Name = "Ghi chú")]
    [DataType(DataType.MultilineText)]
    public string? Comments { get; set; }

    public virtual User? Inspector { get; set; } = null!;

    public virtual InspectionStation? Station { get; set; } = null!;

    public virtual Vehicle? Vehicle { get; set; } = null!;
}
