using EXE_PET_HUB.Application.Interfaces;

namespace EXE_PET_HUB.API.Workers;

public class AppointmentReminderWorker : BackgroundService
{
    private readonly IReminderService _reminderService;
    private readonly ILogger<AppointmentReminderWorker> _logger;
    private static readonly TimeSpan Period = TimeSpan.FromMinutes(30);

    public AppointmentReminderWorker(
        IReminderService reminderService,
        ILogger<AppointmentReminderWorker> logger)
    {
        _reminderService = reminderService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AppointmentReminderWorker started (interval: {Interval}s)", Period.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _reminderService.SyncRemindersAsync();
                await _reminderService.SendPendingRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AppointmentReminderWorker cycle failed");
            }

            await Task.Delay(Period, stoppingToken);
        }
    }
}