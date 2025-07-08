using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Role
{
    [Display(Name = "Mã vai trò")]
    public int RoleId { get; set; }

    [Display(Name = "Tên vai trò")]
    [Required(ErrorMessage = "Vui lòng nhập tên vai trò")]
    [StringLength(50, ErrorMessage = "Tên vai trò không được vượt quá 50 ký tự")]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
