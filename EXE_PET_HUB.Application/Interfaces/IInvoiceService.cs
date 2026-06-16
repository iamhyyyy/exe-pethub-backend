using EXE_PET_HUB.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetAllAsync(string storeId);
        Task<InvoiceDto?> GetByIdAsync(string id, string storeId);
        Task<List<InvoiceDto>> GetAllByCusIDAsync(Guid cusID, string storeId);
        Task<List<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceID, string storeId);
        Task<ResponseInvoiceOfCreateDto> CreateInvoiceAsync(CreateInvoiceDto dto, string storeId);
        Task<bool> MarkAsPaidAsync(string invoiceId, string storeId);
    }
}
