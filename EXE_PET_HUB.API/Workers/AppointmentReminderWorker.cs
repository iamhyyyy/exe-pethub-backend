using EXE_PET_HUB.Application.Interfaces;
using Microsoft.Extensions.Hosting;

public class AppointmentReminderWorker : BackgroundService
{
    private readonly IReminderService _reminderService;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(5); // Định kỳ 5 phút quét 1 lần

    public AppointmentReminderWorker(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Tự động quét và add thêm reminder chưa tồn tại
                await _reminderService.SyncRemindersAsync();

                // 2. Quét xem có lịch nào cần gửi mail thì gửi luôn
                await _reminderService.SendPendingRemindersAsync();
            }
            catch (Exception ex)
            {
                // Thêm log lỗi ở đây để theo dõi nếu Worker bị sập
            }

            // Chờ 5 phút trước khi thực hiện lượt quét tiếp theo
            await Task.Delay(_period, stoppingToken);
        }
    }
}