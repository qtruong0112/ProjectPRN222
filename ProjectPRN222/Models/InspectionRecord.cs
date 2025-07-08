using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionRecord
{
    [Display(Name = "Mã bản ghi")]
    public int RecordId { get; set; }

    [Display(Name = "Mã xe")]
    [Required(ErrorMessage = "Vui lòng chọn xe")]
    public int VehicleId { get; set; }

    [Display(Name = "Mã trạm")]
    [Required(ErrorMessage = "Vui lòng chọn trạm đăng kiểm")]
    public int StationId { get; set; }

    [Display(Name = "Mã thanh tra viên")]
    [Required(ErrorMessage = "Vui lòng chọn thanh tra viên")]
    public int InspectorId { get; set; }

    [Display(Name = "Ngày đăng kiểm")]
    [DataType(DataType.DateTime)]
    public DateTime? InspectionDate { get; set; }

    [Display(Name = "Kết quả")]
    [Required(ErrorMessage = "Vui lòng nhập kết quả đăng kiểm")]
    [StringLength(10, ErrorMessage = "Kết quả không được vượt quá 10 ký tự")]
    [RegularExpression(@"^(Đạt|Không đạt)$", ErrorMessage = "Kết quả chỉ được là: Đạt, Không đạt")]
    public string Result { get; set; } = null!;

    [Display(Name = "Khí thải CO2 (g/km)")]
    [Required(ErrorMessage = "Vui lòng nhập lượng khí thải CO2")]
    [Range(0, 999.99, ErrorMessage = "Lượng khí thải CO2 phải từ 0 đến 999.99 g/km")]
    [RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "Lượng khí thải CO2 phải có định dạng số thập phân hợp lệ")]
    public decimal Co2emission { get; set; }

    [Display(Name = "Khí thải HC (ppm)")]
    [Required(ErrorMessage = "Vui lòng nhập lượng khí thải HC")]
    [Range(0, 999.99, ErrorMessage = "Lượng khí thải HC phải từ 0 đến 999.99 ppm")]
    [RegularExpression(@"^\d{1,3}(\.\d{1,2})?$", ErrorMessage = "Lượng khí thải HC phải có định dạng số thập phân hợp lệ")]
    public decimal Hcemission { get; set; }

    [Display(Name = "Ghi chú")]
    [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
    [DataType(DataType.MultilineText)]
    public string? Comments { get; set; }

    public virtual User Inspector { get; set; } = null!;

    public virtual InspectionStation Station { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
