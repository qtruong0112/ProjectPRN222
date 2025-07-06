using System;
using System.Collections.Generic;

namespace ProjectPRN222.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int OwnerId { get; set; }

    public string PlateNumber { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int ManufactureYear { get; set; }

    public string EngineNumber { get; set; } = null!;

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual User Owner { get; set; } = null!;
}
