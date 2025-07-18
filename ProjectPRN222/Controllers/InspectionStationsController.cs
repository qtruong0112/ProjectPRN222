using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;

namespace ProjectPRN222.Controllers
{
    public class InspectionStationsController : Controller
    {
        private readonly PrnprojectContext _context;

        public InspectionStationsController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: InspectionStations - Tất cả user có thể xem danh sách trạm kiểm định
        [RoleAllow(1, 2, 3, 4, 5)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.InspectionStations.ToListAsync());
        }

        // GET: InspectionStations/Details/5 - Tất cả user có thể xem chi tiết trạm kiểm định
        [RoleAllow(1, 2, 3, 4, 5)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionStation = await _context.InspectionStations
                .FirstOrDefaultAsync(m => m.StationId == id);
            if (inspectionStation == null)
            {
                return NotFound();
            }

            return View(inspectionStation);
        }

        // GET: InspectionStations/Create - Chỉ Admin có thể tạo trạm kiểm định mới
        [RoleAllow(5)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: InspectionStations/Create - Chỉ Admin có thể tạo trạm kiểm định mới
        [RoleAllow(5)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StationId,Name,Address,Phone,Email")] InspectionStation inspectionStation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspectionStation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inspectionStation);
        }

        // GET: InspectionStations/Edit/5 - Admin và InspectionCenter có thể chỉnh sửa
        [RoleAllow(3, 5)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionStation = await _context.InspectionStations.FindAsync(id);
            if (inspectionStation == null)
            {
                return NotFound();
            }
            return View(inspectionStation);
        }

        // POST: InspectionStations/Edit/5 - Admin và InspectionCenter có thể chỉnh sửa
        [RoleAllow(3, 5)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StationId,Name,Address,Phone,Email")] InspectionStation inspectionStation)
        {
            if (id != inspectionStation.StationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspectionStation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionStationExists(inspectionStation.StationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inspectionStation);
        }

        // GET: InspectionStations/Delete/5 - Chỉ Admin có thể xóa trạm kiểm định
        [RoleAllow(5)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionStation = await _context.InspectionStations
                .FirstOrDefaultAsync(m => m.StationId == id);
            if (inspectionStation == null)
            {
                return NotFound();
            }

            return View(inspectionStation);
        }

        // POST: InspectionStations/Delete/5 - Chỉ Admin có thể xóa trạm kiểm định
        [RoleAllow(5)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspectionStation = await _context.InspectionStations.FindAsync(id);
            if (inspectionStation != null)
            {
                try
                {
                    _context.InspectionStations.Remove(inspectionStation);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["DeleteError"] = "Không thể xóa trạm kiểm định vì đã có dữ liệu liên quan.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InspectionStationExists(int id)
        {
            return _context.InspectionStations.Any(e => e.StationId == id);
        }
    }
}
