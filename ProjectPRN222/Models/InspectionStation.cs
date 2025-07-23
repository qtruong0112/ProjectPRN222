using System;
using System.Collections.Generic;

namespace ProjectPRN222.Models;

public partial class InspectionStation
{
    public int StationId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
