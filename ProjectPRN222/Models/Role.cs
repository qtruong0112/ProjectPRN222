using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Role
{
    public int RoleId { get; set; }

    [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
    [Display(Name = "Tên vai trò")]
    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
