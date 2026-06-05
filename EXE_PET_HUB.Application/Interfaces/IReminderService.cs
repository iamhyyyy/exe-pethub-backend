using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IReminderService
    {
        Task SyncRemindersAsync();
        Task SendPendingRemindersAsync();
        Task CancelExpiredAppointmentsAsync();
    }
}
