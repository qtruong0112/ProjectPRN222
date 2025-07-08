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
            var prnprojectContext = _context.InspectionAppointments.Include(i => i.Station).Include(i => i.User).Include(i => i.Vehicle);
            return View(await prnprojectContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "PlateNumber");
            var model = new InspectionAppointment { Status = "Pending", AppointmentDate = DateTime.Now };
            return View(model);
        }

        // POST: InspectionAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: InspectionAppointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,VehicleId,UserId,StationId,AppointmentDate,Status,Note")] InspectionAppointment inspectionAppointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspectionAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // DEBUG: In lỗi ModelState
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Re-fill dropdowns if model state is invalid
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", inspectionAppointment.StationId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullName", inspectionAppointment.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "PlateNumber", inspectionAppointment.VehicleId);
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
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "StationId", inspectionAppointment.StationId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", inspectionAppointment.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", inspectionAppointment.VehicleId);
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
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "StationId", inspectionAppointment.StationId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", inspectionAppointment.UserId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", inspectionAppointment.VehicleId);
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
