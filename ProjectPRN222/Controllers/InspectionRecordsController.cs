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
    public class InspectionRecordsController : Controller
    {
        private readonly PrnprojectContext _context;

        public InspectionRecordsController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: InspectionRecords
        public async Task<IActionResult> Index()
        {
            var prnprojectContext = _context.InspectionRecords.Include(i => i.Inspector).Include(i => i.Station).Include(i => i.Vehicle).ThenInclude(v => v.Owner);
            return View(await prnprojectContext.ToListAsync());
        }

        // GET: InspectionRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionRecord = await _context.InspectionRecords
                .Include(i => i.Inspector)
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.RecordId == id);
            if (inspectionRecord == null)
            {
                return NotFound();
            }

            return View(inspectionRecord);
        }

        // GET: InspectionRecords/Create
        public IActionResult Create()
        {
            ViewData["InspectorId"] = new SelectList(_context.Users.Where(u => u.RoleId == 2), "UserId", "FullName"); // Worker
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name");

            // Hiển thị Vehicle kèm tên chủ xe
            var vehicles = _context.Vehicles.Include(v => v.Owner).ToList();
            ViewData["VehicleId"] = new SelectList(vehicles.Select(v => new
            {
                VehicleId = v.VehicleId,
                Display = v.PlateNumber + " - " + v.Owner.FullName
            }), "VehicleId", "Display");

            return View();
        }


        // POST: InspectionRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecordId,VehicleId,StationId,InspectorId,InspectionDate,Result,Co2emission,Hcemission,Comments")] InspectionRecord inspectionRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspectionRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "UserId", "UserId", inspectionRecord.InspectorId);
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "StationId", inspectionRecord.StationId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", inspectionRecord.VehicleId);
            return View(inspectionRecord);
        }

        // GET: InspectionRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionRecord = await _context.InspectionRecords.FindAsync(id);
            if (inspectionRecord == null)
            {
                return NotFound();
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "UserId", "UserId", inspectionRecord.InspectorId);
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "StationId", inspectionRecord.StationId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", inspectionRecord.VehicleId);
            return View(inspectionRecord);
        }

        // POST: InspectionRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordId,VehicleId,StationId,InspectorId,InspectionDate,Result,Co2emission,Hcemission,Comments")] InspectionRecord inspectionRecord)
        {
            if (id != inspectionRecord.RecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspectionRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionRecordExists(inspectionRecord.RecordId))
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
            ViewData["InspectorId"] = new SelectList(_context.Users, "UserId", "UserId", inspectionRecord.InspectorId);
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "StationId", inspectionRecord.StationId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "VehicleId", inspectionRecord.VehicleId);
            return View(inspectionRecord);
        }

        // GET: InspectionRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspectionRecord = await _context.InspectionRecords
                .Include(i => i.Inspector)
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                .FirstOrDefaultAsync(m => m.RecordId == id);
            if (inspectionRecord == null)
            {
                return NotFound();
            }

            return View(inspectionRecord);
        }

        // POST: InspectionRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspectionRecord = await _context.InspectionRecords.FindAsync(id);
            if (inspectionRecord != null)
            {
                _context.InspectionRecords.Remove(inspectionRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionRecordExists(int id)
        {
            return _context.InspectionRecords.Any(e => e.RecordId == id);
        }
    }
}
