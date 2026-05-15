using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<List<Invoice>> GetAllInvoicesWithDetailsAsync();
        Task<List<Invoice>> GetAllInvoicesDetailsByCusIDAsync(Guid customerID);
        Task<List<InvoiceDetail>> GetDetailsAsync(string InvoiceID);
        Task<Invoice> GetInvoiceWithDetailsAsync(string invoiceId);
    }
}
