using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Vehicle
{
    [Display(Name = "Mã xe")]
    public int VehicleId { get; set; }

    [Display(Name = "Mã chủ sở hữu")]
    [Required(ErrorMessage = "Vui lòng chọn chủ sở hữu")]
    public int OwnerId { get; set; }

    [Display(Name = "Biển số xe")]
    [Required(ErrorMessage = "Vui lòng nhập biển số xe")]
    [StringLength(15, ErrorMessage = "Biển số xe không được vượt quá 15 ký tự")]
    [RegularExpression(@"^[0-9]{2}[A-Z]{1,2}-[0-9]{3,4}\.[0-9]{2}$|^[0-9]{2}[A-Z]{1,2}[0-9]{3,4}$", 
        ErrorMessage = "Biển số xe không đúng định dạng (VD: 30A-12345 hoặc 30A12345)")]
    public string PlateNumber { get; set; } = null!;

    [Display(Name = "Hãng xe")]
    [Required(ErrorMessage = "Vui lòng nhập hãng xe")]
    [StringLength(50, ErrorMessage = "Tên hãng xe không được vượt quá 50 ký tự")]
    public string Brand { get; set; } = null!;

    [Display(Name = "Dòng xe")]
    [Required(ErrorMessage = "Vui lòng nhập dòng xe")]
    [StringLength(50, ErrorMessage = "Tên dòng xe không được vượt quá 50 ký tự")]
    public string Model { get; set; } = null!;

    [Display(Name = "Năm sản xuất")]
    [Required(ErrorMessage = "Vui lòng nhập năm sản xuất")]
    [Range(1990, 2025, ErrorMessage = "Năm sản xuất phải từ 1990 đến 2025")]
    public int ManufactureYear { get; set; }

    [Display(Name = "Số động cơ")]
    [Required(ErrorMessage = "Vui lòng nhập số động cơ")]
    [StringLength(100, ErrorMessage = "Số động cơ không được vượt quá 100 ký tự")]
    public string EngineNumber { get; set; } = null!;

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User? Owner { get; set; }
}
