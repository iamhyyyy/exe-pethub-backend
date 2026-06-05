using EXE_PET_HUB.Application.Interfaces;

namespace EXE_PET_HUB.API.Workers;

public class AppointmentStatusWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppointmentStatusWorker> _logger;
    private static readonly TimeSpan Period = TimeSpan.FromMinutes(30);

    public AppointmentStatusWorker(
        IServiceProvider serviceProvider,
        ILogger<AppointmentStatusWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AppointmentStatusWorker started (interval: {Interval}m)", Period.TotalMinutes);

        // Sử dụng PeriodicTimer (tính năng mới từ .NET 6 trở lên, chạy mượt và chuẩn xác hơn Task.Delay)
        using PeriodicTimer timer = new PeriodicTimer(Period);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Bắt đầu quét và xử lý lịch hẹn quá hạn...");

                // Tạo scope để tránh lỗi DbContext (vì BackgroundService là Singleton)
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                    await reminderService.CancelExpiredAppointmentsAsync();
                }

                _logger.LogInformation("Quét lịch hẹn hoàn tất.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AppointmentStatusWorker cycle failed");
            }
        }
    }
}