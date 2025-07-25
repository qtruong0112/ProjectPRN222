using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;
using static System.Collections.Specialized.BitVector32;

namespace ProjectPRN222.Controllers
{
    [RoleAllow(5, 3)]
    public class StatisticalsController : Controller
    {
        private readonly PrnprojectContext _context;

        public StatisticalsController(PrnprojectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int stationId = 0)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentUser = await _context.Users.FindAsync(currentUserId);

            // Nếu user là InspectionCenter (role 3), chỉ cho phép xem thống kê của trạm mình
            if (currentUser != null && currentUser.RoleId == 3)
            {
                if (currentUser.StationId.HasValue)
                {
                    stationId = currentUser.StationId.Value;
                }
                else
                {
                    return BadRequest("Bạn chưa được phân công vào trạm nào.");
                }
            }

            // Nếu không có stationId, hiển thị form chọn trạm (chỉ cho Admin)
            if (stationId == 0)
            {
                ViewBag.Stations = await _context.InspectionStations.ToListAsync();
                return View("SelectStation");
            }

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
