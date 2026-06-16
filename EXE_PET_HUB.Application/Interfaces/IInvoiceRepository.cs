using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IInvoiceRepository : IStoreGenericRepository<Invoice>
    {
        Task<List<Invoice>> GetAllInvoicesWithStoreIDAsync(string storeId);
        Task<List<Invoice>> GetAllInvoicesByCusIDAndWithStoreIDAsync(Guid customerID, string storeId);
        Task<List<InvoiceDetail>> GetDetailsAsync(string InvoiceID, string storeId);
        Task<Invoice> GetInvoiceByIdAndWithStoreIDAsync(string invoiceId, string storeId);
        Task<Invoice> GetInvoiceByOrderCodeAsync(long orderCode);
    }
}
