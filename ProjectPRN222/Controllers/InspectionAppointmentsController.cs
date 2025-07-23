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
    [RoleAllow(5, 3, 2, 1)]
    public class InspectionAppointmentsController : Controller
    {
        private readonly PrnprojectContext _context;

        public InspectionAppointmentsController(PrnprojectContext context)
        {
            _context = context;
        }


        // GET: InspectionAppointments
        public async Task<IActionResult> Index()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUser = await _context.Users.FindAsync(currentUserId);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<InspectionAppointment> appointments;

            if (currentUser.RoleId == 2) // Admin hoặc nhân viên
            {
                appointments = await _context.InspectionAppointments
                    .Include(i => i.Station)
                    .Include(i => i.User)
                    .Include(i => i.Vehicle)
                    .ToListAsync();
            }
            else
            {
                appointments = await _context.InspectionAppointments
                    .Include(i => i.Station)
                    .Include(i => i.User)
                    .Include(i => i.Vehicle)
                    .Where(i => i.UserId == currentUserId)
                    .ToListAsync();
            }



            return View(appointments);
        }

        // GET: InspectionAppointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionAppointment = await _context.InspectionAppointments
                .Include(i => i.Station)
                .Include(i => i.User)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (inspectionAppointment == null)
            {
                return NotFound();
            }

            return View(inspectionAppointment);
        }

        // GET: InspectionAppointments/Create
        // GET: InspectionAppointments/Create
        // GET: InspectionAppointments/Create
        public IActionResult Create()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var stations = _context.InspectionStations.ToList();
            var userVehicles = _context.Vehicles.Where(v => v.OwnerId == currentUserId).ToList();

            ViewData["StationId"] = new SelectList(stations, "StationId", "Name");
            ViewData["VehicleId"] = new SelectList(userVehicles, "VehicleId", "PlateNumber");

            var model = new InspectionAppointment
            {
                Status = "Pending",
                AppointmentDate = DateTime.Now,
                UserId = currentUserId.Value
            };

            return View(model);
        }

        // POST: InspectionAppointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,StationId,AppointmentDate,Note,UserId,Status")] InspectionAppointment inspectionAppointment)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (inspectionAppointment.UserId == 0)
                inspectionAppointment.UserId = currentUserId.Value;

            if (string.IsNullOrEmpty(inspectionAppointment.Status))
                inspectionAppointment.Status = "Pending";

            if (inspectionAppointment.VehicleId == 0)
            {
                ModelState.AddModelError("VehicleId", "Vui lòng chọn xe");
            }

            if (inspectionAppointment.StationId == 0)
            {
                ModelState.AddModelError("StationId", "Vui lòng chọn trạm đăng kiểm");
            }

            if (inspectionAppointment.AppointmentDate == default(DateTime) || inspectionAppointment.AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("AppointmentDate", "Vui lòng chọn ngày hẹn hợp lệ");
            }

            ModelState.Remove("Station");
            ModelState.Remove("User");
            ModelState.Remove("Vehicle");

            if (ModelState.IsValid)
            {
                var vehicleExists = await _context.Vehicles.AnyAsync(v => v.VehicleId == inspectionAppointment.VehicleId);
                var stationExists = await _context.InspectionStations.AnyAsync(s => s.StationId == inspectionAppointment.StationId);
                var userExists = await _context.Users.AnyAsync(u => u.UserId == inspectionAppointment.UserId);

                if (!vehicleExists)
                    ModelState.AddModelError("VehicleId", "Vehicle not found");
                if (!stationExists)
                    ModelState.AddModelError("StationId", "Station not found");
                if (!userExists)
                    ModelState.AddModelError("UserId", "User not found");

                if (vehicleExists && stationExists && userExists)
                {
                    _context.Add(inspectionAppointment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            var stations = await _context.InspectionStations.ToListAsync();
            var userVehicles = await _context.Vehicles.Where(v => v.OwnerId == currentUserId).ToListAsync();

            ViewData["StationId"] = new SelectList(stations, "StationId", "Name", inspectionAppointment.StationId);
            ViewData["VehicleId"] = new SelectList(userVehicles, "VehicleId", "PlateNumber", inspectionAppointment.VehicleId);

            return View(inspectionAppointment);
        }



        // GET: InspectionAppointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionAppointment = await _context.InspectionAppointments.FindAsync(id);
            if (inspectionAppointment == null)
            {
                return NotFound();
            }
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", inspectionAppointment.StationId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", inspectionAppointment.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "PlateNumber", inspectionAppointment.VehicleId);
            return View(inspectionAppointment);
        }

        // POST: InspectionAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,VehicleId,UserId,StationId,AppointmentDate,Status,Note")] InspectionAppointment inspectionAppointment)
        {
            if (id != inspectionAppointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspectionAppointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionAppointmentExists(inspectionAppointment.AppointmentId))
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
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", inspectionAppointment.StationId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", inspectionAppointment.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "PlateNumber", inspectionAppointment.VehicleId);
            return View(inspectionAppointment);
        }

        // GET: InspectionAppointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionAppointment = await _context.InspectionAppointments
                .Include(i => i.Station)
                .Include(i => i.User)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (inspectionAppointment == null)
            {
                return NotFound();
            }

            return View(inspectionAppointment);
        }

        // POST: InspectionAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspectionAppointment = await _context.InspectionAppointments.FindAsync(id);
            if (inspectionAppointment != null)
            {
                _context.InspectionAppointments.Remove(inspectionAppointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionAppointmentExists(int id)
        {
            return _context.InspectionAppointments.Any(e => e.AppointmentId == id);
        }
    }
}