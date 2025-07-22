using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;
using ProjectPRN222.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ProjectPRN222.Controllers
{
    
    public class InspectionRecordsController : Controller
    {
        private readonly PrnprojectContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public InspectionRecordsController(PrnprojectContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Tạo thông báo đơn giản - sử dụng NotificationsController
        private async Task CreateNotificationAsync(int userId, string message)
        {
            var notificationController = new NotificationsController(_context, _hubContext);
            await notificationController.CreateNotification(userId, message);
        }

        // Gửi thông báo khi có kết quả kiểm định mới
        private async Task SendInspectionResultNotificationAsync(int vehicleOwnerId, string vehicleInfo, string result)
        {
            var message = $"Kết quả kiểm định cho xe {vehicleInfo}: {result}";
            await CreateNotificationAsync(vehicleOwnerId, message);
        }

        // GET: InspectionRecords
   
        public async Task<IActionResult> Index(
            int StationId = -1,
            string PlateNumber = null,
            string Result = null,
            DateTime? FromDate = null,
            DateTime? ToDate = null)
        {
            var query = _context.InspectionRecords
                .Include(i => i.Inspector)
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .AsQueryable();

            if (StationId > 0)
            {
                query = query.Where(i => i.StationId == StationId);
            }

            if (!string.IsNullOrWhiteSpace(PlateNumber))
            {
                query = query.Where(i => i.Vehicle.PlateNumber.Contains(PlateNumber));
            }

            if (!string.IsNullOrWhiteSpace(Result))
            {
                query = query.Where(i => i.Result == Result);
            }

            if (FromDate.HasValue)
            {
                query = query.Where(i => i.InspectionDate >= FromDate.Value);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(i => i.InspectionDate <= ToDate.Value);
            }

            ViewBag.StationId = new SelectList(_context.InspectionStations.ToList(), "StationId", "Name", StationId);
            ViewBag.PlateNumber = PlateNumber;
            ViewBag.ResultList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "-- Tất cả kết quả --", Value = "" },
                new SelectListItem { Text = "Pass", Value = "Pass" },
                new SelectListItem { Text = "Fail", Value = "Fail" }
}           , "Value", "Text", Result);

            ViewBag.FromDate = FromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate?.ToString("yyyy-MM-dd");

            return View(await query.ToListAsync());
        }

        // GET: InspectionRecords/ForPolice

        [RoleAllow(4, 5)]
        public async Task<IActionResult> ForPolice(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                ViewBag.Message = "Vui lòng nhập biển số xe để tra cứu.";
                return View(null);
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);

            if (vehicle == null)
            {
                ViewBag.Message = "Không tìm thấy phương tiện với biển số này.";
                return View(null);
            }

            var latestRecord = await _context.InspectionRecords
                .Where(r => r.VehicleId == vehicle.VehicleId)
                .OrderByDescending(r => r.InspectionDate)
                .FirstOrDefaultAsync();

            string status = "Không vi phạm";

            if (latestRecord == null)
            {
                status = "Chưa từng đăng kiểm - Vi phạm";
            }
            else if (latestRecord.Result == "Fail")
            {
                status = "Đăng kiểm gần nhất là 'Fail' - Vi phạm";
            }
            else if (latestRecord.Result == "Pass" &&
                latestRecord.InspectionDate.HasValue &&
                latestRecord.InspectionDate.Value.AddMonths(6) < DateTime.Now)
            {
                status = "Đã hết hạn đăng kiểm - Vi phạm";
            }

            ViewBag.Vehicle = vehicle;

            ViewBag.LatestRecord = latestRecord;
            ViewBag.Status = status;

            return View();
        }

        public async Task<IActionResult> ForOwnerVehicle(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                ViewBag.Message = "Vui lòng nhập biển số xe để tra cứu.";
                return View(null);
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);

            if (vehicle == null)
            {
                ViewBag.Message = "Không tìm thấy phương tiện với biển số này.";
                return View(null);
            }

            var records = await _context.InspectionRecords
                .Include(r => r.Inspector)
                .Include(r => r.Station)
                .Include(r => r.Vehicle).ThenInclude(v => v.Owner)
                .Where(r => r.VehicleId == vehicle.VehicleId)
                .OrderByDescending(r => r.InspectionDate)
                .ToListAsync();

            if (records == null || records.Count == 0)
            {
                ViewBag.Message = "Xe chưa từng đăng kiểm.";
                return View(null);
            }

            ViewBag.Vehicle = vehicle;
            return View(records);
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

    
        public async Task<IActionResult> CreateFromAppointment(int appointmentId)
        {
            var appointment = await _context.InspectionAppointments
                .Include(a => a.User)
                .Include(a => a.Station)
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return NotFound();

            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "VehicleId", "PlateNumber", appointment.VehicleId);
            ViewData["StationId"] = new SelectList(_context.InspectionStations, "StationId", "Name", appointment.StationId);
            ViewData["InspectorId"] = new SelectList(_context.Users.Where(u => u.RoleId == 2), "UserId", "FullName");

            var record = new InspectionRecord
            {
                VehicleId = appointment.VehicleId,
                StationId = appointment.StationId,
                InspectionDate = appointment.AppointmentDate
               
            };

           if( appointmentId > 0)
            {
                appointment = await _context.InspectionAppointments.FindAsync(appointmentId);
                if (appointment != null)
                {
                    appointment.Status = "Completed"; 
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
            }

            return View("Create", record);
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

                // Gửi thông báo kết quả kiểm định cho chủ xe
                try
                {
                    var vehicle = await _context.Vehicles
                        .Include(v => v.Owner)
                        .FirstOrDefaultAsync(v => v.VehicleId == inspectionRecord.VehicleId);
                    
                    if (vehicle != null && vehicle.Owner != null)
                    {
                        var vehicleInfo = $"{vehicle.PlateNumber} ({vehicle.Brand} {vehicle.Model})";
                        await SendInspectionResultNotificationAsync(
                            vehicle.OwnerId, 
                            vehicleInfo, 
                            inspectionRecord.Result ?? "Đang xử lý"
                        );
                    }
                }
                catch (Exception ex)
                {
                    // Log error nhưng không làm gián đoạn process
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }

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
