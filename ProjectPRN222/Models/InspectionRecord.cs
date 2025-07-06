using System;
using System.Collections.Generic;

namespace ProjectPRN222.Models;

public partial class InspectionRecord
{
    public int RecordId { get; set; }

    public int VehicleId { get; set; }

    public int StationId { get; set; }

    public int InspectorId { get; set; }

    public DateTime? InspectionDate { get; set; }

    public string Result { get; set; } = null!;

    public decimal Co2emission { get; set; }

    public decimal Hcemission { get; set; }

    public string? Comments { get; set; }

    public virtual User Inspector { get; set; } = null!;

    public virtual InspectionStation Station { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
