using Microsoft.EntityFrameworkCore;
using ProjectPRN222.Models;

namespace ProjectPRN222.Services
{
    public class InspectionReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InspectionReminderService> _logger;

        public InspectionReminderService(IServiceProvider serviceProvider, ILogger<InspectionReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckInspectionReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in InspectionReminderService");
                }

                // Chạy mỗi 24 giờ
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CheckInspectionReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PrnprojectContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

            // Lấy tất cả xe cần kiểm tra
            var vehicles = await context.Vehicles
                .Include(v => v.Owner)
                .Include(v => v.InspectionRecords)
                .ToListAsync();

            foreach (var vehicle in vehicles)
            {
                try
                {
                    // Tìm bản ghi kiểm định gần nhất
                    var latestRecord = vehicle.InspectionRecords
                        .OrderByDescending(r => r.InspectionDate)
                        .FirstOrDefault();

                    DateTime? nextInspectionDue = null;

                    if (latestRecord == null)
                    {
                        // Xe chưa từng kiểm định - cần kiểm định ngay
                        nextInspectionDue = DateTime.Now.AddDays(7); // Cho 7 ngày grace period
                    }
                    else if (latestRecord.Result == "Pass" && latestRecord.InspectionDate.HasValue)
                    {
                        // Xe đã pass - hạn kiểm định sau 6 tháng
                        nextInspectionDue = latestRecord.InspectionDate.Value.AddMonths(6);
                    }
                    else if (latestRecord.Result == "Fail" && latestRecord.InspectionDate.HasValue)
                    {
                        // Xe fail - cần kiểm định lại trong 1 tháng
                        nextInspectionDue = latestRecord.InspectionDate.Value.AddMonths(1);
                    }

                    if (nextInspectionDue.HasValue)
                    {
                        var daysUntilDue = (nextInspectionDue.Value - DateTime.Now).Days;
                        
                        // Gửi thông báo nhắc nhở khi còn 30 ngày, 15 ngày, 7 ngày và 1 ngày
                        if (daysUntilDue == 30 || daysUntilDue == 15 || daysUntilDue == 7 || daysUntilDue == 1)
                        {
                            var vehicleInfo = $"{vehicle.PlateNumber} ({vehicle.Brand} {vehicle.Model})";
                            await notificationService.SendInspectionReminderNotificationAsync(
                                vehicle.OwnerId, 
                                vehicleInfo, 
                                nextInspectionDue.Value
                            );

                            _logger.LogInformation($"Sent inspection reminder for vehicle {vehicle.PlateNumber} to user {vehicle.OwnerId}");
                        }
                        
                        // Gửi thông báo quá hạn
                        else if (daysUntilDue < 0)
                        {
                            var vehicleInfo = $"{vehicle.PlateNumber} ({vehicle.Brand} {vehicle.Model})";
                            var overdueDays = Math.Abs(daysUntilDue);
                            
                            await notificationService.CreateNotificationAsync(
                                vehicle.OwnerId,
                                $"Xe {vehicleInfo} đã quá hạn kiểm định {overdueDays} ngày. Vui lòng kiểm định ngay lập tức!",
                                "Cảnh báo quá hạn kiểm định"
                            );

                            _logger.LogWarning($"Vehicle {vehicle.PlateNumber} is overdue by {overdueDays} days");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing vehicle {vehicle.PlateNumber}");
                }
            }

            _logger.LogInformation("Completed inspection reminder check");
        }
    }
} 