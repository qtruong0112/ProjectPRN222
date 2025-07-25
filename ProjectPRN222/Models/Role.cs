using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Role
{
    [Key]
    [Display(Name = "Mã vai trò")]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
    [Display(Name = "Tên vai trò")]
    [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
    public string RoleName { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
