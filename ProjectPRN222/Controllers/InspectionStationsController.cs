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
    public class InspectionStationsController : Controller
    {
        private readonly PrnprojectContext _context;

        public InspectionStationsController(PrnprojectContext context)
        {
            _context = context;
        }

        // GET: InspectionStations
        public async Task<IActionResult> Index()
        {
            return View(await _context.InspectionStations.ToListAsync());
        }

        // GET: InspectionStations/Details/5
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

        // GET: InspectionStations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InspectionStations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: InspectionStations/Edit/5
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

        // POST: InspectionStations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: InspectionStations/Delete/5
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

        // POST: InspectionStations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspectionStation = await _context.InspectionStations.FindAsync(id);
            if (inspectionStation != null)
            {
                _context.InspectionStations.Remove(inspectionStation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionStationExists(int id)
        {
            return _context.InspectionStations.Any(e => e.StationId == id);
        }
    }
}
