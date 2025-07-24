using System.ComponentModel.DataAnnotations;

namespace ProjectPRN222.Models
{
    public class InspectionResultStatsViewModel
    {
        [Display(Name = "Mã trạm")]
        public int StationId { get; set; }

        [Display(Name = "Tên trạm")]
        public string StationName { get; set; } = null!;

        [Display(Name = "Kết quả")]
        public string Result { get; set; } = null!;

        [Display(Name = "Tổng số xe")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TotalVehicles { get; set; }
    }
}

