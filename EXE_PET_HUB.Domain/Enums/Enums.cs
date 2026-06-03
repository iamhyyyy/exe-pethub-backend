using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Domain.Enums
{
    public enum UserRole
    {
        Customer,
        Manager,
        Admin
    }

    public enum PlanType
    {
        Free,
        Premium
    }

    public enum AppointmentStatus
    {
        Confirmed,
        Completed,
        Cancelled
    }

    public enum ItemType
    {
        Service,
        Product,
        Plan 
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Cancelled,
        Failed
    }

    public enum ReminderStatus
    {
        Pending,
        Sent,
        Failed
    }

    public enum InvoiceStatus
    {
        Pending,
        Paid,
        Cancelled,
        Failed
    }
}
