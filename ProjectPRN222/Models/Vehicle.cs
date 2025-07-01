using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "Owner is required")]
    [Display(Name = "Owner")]
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "Plate number is required")]
    [StringLength(15, ErrorMessage = "Plate number cannot be longer than 15 characters")]
    [Display(Name = "Plate Number")]
    public string PlateNumber { get; set; } = null!;

    [Required(ErrorMessage = "Brand is required")]
    [StringLength(50, ErrorMessage = "Brand cannot be longer than 50 characters")]
    [Display(Name = "Brand")]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Model is required")]
    [StringLength(50, ErrorMessage = "Model cannot be longer than 50 characters")]
    [Display(Name = "Model")]
    public string Model { get; set; } = null!;

    [Required(ErrorMessage = "Manufacture year is required")]
    [Range(1900, 2024, ErrorMessage = "Manufacture year must be between 1900 and 2024")]
    [Display(Name = "Manufacture Year")]
    public int ManufactureYear { get; set; }

    [Required(ErrorMessage = "Engine number is required")]
    [StringLength(100, ErrorMessage = "Engine number cannot be longer than 100 characters")]
    [Display(Name = "Engine Number")]
    public string EngineNumber { get; set; } = null!;

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User Owner { get; set; } = null!;
}
