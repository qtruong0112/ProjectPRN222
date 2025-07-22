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

    [RoleAllow(5, 3)]
    public class InspectionStationsController : Controller
    {
        private readonly PrnprojectContext _context;

        public InspectionStationsController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: InspectionStations - Với filter  
        public async Task<IActionResult> Index(string name = null, string address = null, string email = null)
        {
            var query = _context.InspectionStations.AsQueryable();

            // Apply filters - case insensitive
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.Name.ToLower().Contains(name.Trim().ToLower()));
            
            if (!string.IsNullOrWhiteSpace(address))
                query = query.Where(s => s.Address.ToLower().Contains(address.Trim().ToLower()));
            
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(s => s.Email.ToLower().Contains(email.Trim().ToLower()));

            // ViewBag for retaining filter values
            ViewBag.Name = name;
            ViewBag.Address = address;
            ViewBag.Email = email;

            return View(await query.ToListAsync());
        }

        // GET: InspectionStations/Details/5 - Tất cả user có thể xem chi tiết trạm kiểm định
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
