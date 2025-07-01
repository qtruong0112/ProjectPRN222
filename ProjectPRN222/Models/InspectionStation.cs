using System;
using System.Collections.Generic;

namespace ProjectPRN222.Models;

public partial class InspectionStation
{
    public int StationId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();
}
