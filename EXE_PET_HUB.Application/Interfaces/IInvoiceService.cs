using EXE_PET_HUB.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetAllAsync();
        Task<InvoiceDto?> GetByIdAsync(string id);
        Task<List<InvoiceDto>> GetAllByCusIDAsync(Guid cusID);
        Task<List<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceID);
        Task<ResponseInvoiceOfCreateDto> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<bool> MarkAsPaidAsync(string invoiceId);
    }
}
