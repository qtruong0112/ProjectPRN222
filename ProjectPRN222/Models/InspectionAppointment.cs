using System;
using System.Collections.Generic;

namespace ProjectPRN222.Models;

public partial class InspectionAppointment
{
    public int AppointmentId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public int StationId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public virtual InspectionStation? Station { get; set; } = null!;

    public virtual User? User { get; set; } = null!;

    public virtual Vehicle? Vehicle { get; set; } = null!;
}
