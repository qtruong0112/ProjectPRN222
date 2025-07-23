namespace ProjectPRN222.Models
{
    public class InspectionResultStatsViewModel
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = null!;
        public string Result { get; set; } = null!;
        public int TotalVehicles { get; set; }
    }
}

