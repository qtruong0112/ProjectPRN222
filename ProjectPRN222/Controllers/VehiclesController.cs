using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;

namespace ProjectPRN222.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly PrnprojectContext _context;

        public VehiclesController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: Vehicles - Tất cả user có thể xem danh sách xe
        [RoleAllow(1, 2, 3, 4, 5)]
        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            var query = _context.Vehicles.Include(v => v.Owner).AsQueryable();
            
            // Nếu là chủ xe, chỉ hiển thị xe của mình
            if (currentRoleId == 1 && currentUserId.HasValue)
            {
                query = query.Where(v => v.OwnerId == currentUserId.Value);
            }
            
            var vehicles = await query.ToListAsync();
            return View(vehicles);
        }

        // GET: Vehicles/Details/5 - Tất cả user có thể xem chi tiết xe
        [RoleAllow(1, 2, 3, 4, 5)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Nếu là chủ xe, chỉ có thể xem xe của mình
            if (currentRoleId == 1 && currentUserId.HasValue && vehicle.OwnerId != currentUserId.Value)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create - Chủ xe và Admin có thể tạo xe mới
        [RoleAllow(1, 5)]
        public IActionResult Create()
        {
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            if (currentRoleId == 1)
            {
                // Chủ xe chỉ có thể tạo xe cho mình
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                ViewData["OwnerId"] = new SelectList(_context.Users.Where(u => u.UserId == currentUserId), "UserId", "FullName");
            }
            else
            {
                // Admin có thể tạo xe cho bất kỳ ai
                ViewData["OwnerId"] = new SelectList(_context.Users, "UserId", "FullName");
            }
            
            return View();
        }

        // POST: Vehicles/Create - Chủ xe và Admin có thể tạo xe mới
        [RoleAllow(1, 5)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,OwnerId,PlateNumber,Brand,Model,ManufactureYear,EngineNumber")] Vehicle vehicle)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Nếu là chủ xe, chỉ có thể tạo xe cho mình
            if (currentRoleId == 1 && currentUserId.HasValue && vehicle.OwnerId != currentUserId.Value)
            {
                ModelState.AddModelError("OwnerId", "Bạn chỉ có thể tạo xe cho chính mình.");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Reload ViewData dựa trên role
            if (currentRoleId == 1)
            {
                ViewData["OwnerId"] = new SelectList(_context.Users.Where(u => u.UserId == currentUserId), "UserId", "FullName", vehicle.OwnerId);
            }
            else
            {
                ViewData["OwnerId"] = new SelectList(_context.Users, "UserId", "FullName", vehicle.OwnerId);
            }
            
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5 - Chủ xe và Admin có thể chỉnh sửa xe
        [RoleAllow(1, 5)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Nếu là chủ xe, chỉ có thể edit xe của mình
            if (currentRoleId == 1 && currentUserId.HasValue && vehicle.OwnerId != currentUserId.Value)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            
            if (currentRoleId == 1)
            {
                // Chủ xe không thể thay đổi owner
                ViewData["OwnerId"] = new SelectList(_context.Users.Where(u => u.UserId == currentUserId), "UserId", "FullName", vehicle.OwnerId);
            }
            else
            {
                // Admin có thể thay đổi owner
                ViewData["OwnerId"] = new SelectList(_context.Users, "UserId", "FullName", vehicle.OwnerId);
            }
            
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5 - Chủ xe và Admin có thể chỉnh sửa xe
        [RoleAllow(1, 5)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,OwnerId,PlateNumber,Brand,Model,ManufactureYear,EngineNumber")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Lấy thông tin xe gốc để kiểm tra ownership
            var originalVehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == id);
            if (originalVehicle == null)
            {
                return NotFound();
            }
            
            // Nếu là chủ xe, chỉ có thể edit xe của mình và không thể thay đổi owner
            if (currentRoleId == 1 && currentUserId.HasValue)
            {
                if (originalVehicle.OwnerId != currentUserId.Value)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }
                
                if (vehicle.OwnerId != currentUserId.Value)
                {
                    ModelState.AddModelError("OwnerId", "Bạn không thể thay đổi chủ sở hữu xe.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
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
            
            // Reload ViewData dựa trên role
            if (currentRoleId == 1)
            {
                ViewData["OwnerId"] = new SelectList(_context.Users.Where(u => u.UserId == currentUserId), "UserId", "FullName", vehicle.OwnerId);
            }
            else
            {
                ViewData["OwnerId"] = new SelectList(_context.Users, "UserId", "FullName", vehicle.OwnerId);
            }
            
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5 - Chủ xe và Admin có thể xóa xe
        [RoleAllow(1, 5)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(m => m.VehicleId == id);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Nếu là chủ xe, chỉ có thể delete xe của mình
            if (currentRoleId == 1 && currentUserId.HasValue && vehicle.OwnerId != currentUserId.Value)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5 - Chủ xe và Admin có thể xóa xe
        [RoleAllow(1, 5)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            var currentRoleId = HttpContext.Session.GetInt32("RoleId");
            
            // Nếu là chủ xe, chỉ có thể delete xe của mình
            if (currentRoleId == 1 && currentUserId.HasValue && vehicle.OwnerId != currentUserId.Value)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            try
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["DeleteError"] = "Không thể xoá xe vì đã tồn tại trong lịch sử đăng ký kiểm định.";
                return RedirectToAction("Index", "Vehicles");
            }
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
