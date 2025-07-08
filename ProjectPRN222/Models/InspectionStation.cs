using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class InspectionStation
{
    [Display(Name = "Mã trạm")]
    public int StationId { get; set; }

    [Display(Name = "Tên trạm đăng kiểm")]
    [Required(ErrorMessage = "Vui lòng nhập tên trạm đăng kiểm")]
    [StringLength(100, ErrorMessage = "Tên trạm không được vượt quá 100 ký tự")]
    public string Name { get; set; } = null!;

    [Display(Name = "Địa chỉ")]
    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
    public string Address { get; set; } = null!;

    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [StringLength(10, ErrorMessage = "Số điện thoại phải có 10 chữ số")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    public string Email { get; set; } = null!;

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();
}
