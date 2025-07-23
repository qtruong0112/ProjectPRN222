using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;
using static System.Collections.Specialized.BitVector32;

namespace ProjectPRN222.Controllers
{
    public class StatisticalsController : Controller
    {
        private readonly PrnprojectContext _context;

        public StatisticalsController(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int stationId)
        {
            var station = await _context.InspectionStations
                                .FirstOrDefaultAsync(s => s.StationId == stationId);

            if (station == null)
            {
                return NotFound("Trạm không tồn tại.");
            }

            var stats = await _context.InspectionRecords
                .Include(r => r.Station)
                .Where(r => r.StationId == stationId)
                .GroupBy(r => new { r.StationId, r.Station.Name, r.Result })
                .Select(g => new InspectionResultStatsViewModel
                {
                    StationId = g.Key.StationId,
                    StationName = g.Key.Name,
                    Result = g.Key.Result,
                    TotalVehicles = g.Count()
                })
                .ToListAsync();

            ViewBag.StationName = station.Name;

            return View(stats);
        }
    }
}
