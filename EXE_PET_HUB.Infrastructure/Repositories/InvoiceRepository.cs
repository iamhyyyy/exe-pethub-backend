using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
//using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class InvoiceRepository : StoreGenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Invoice>> GetAllInvoicesWithStoreIDAsync(string storeId)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Where(i =>  i.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAndWithStoreIDAsync(string invoiceId, string storeId)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Where(i => i.Id == invoiceId && i.StoreId == storeId)
                .SingleOrDefaultAsync();
        }
        public async Task<List<Invoice>> GetAllInvoicesByCusIDAndWithStoreIDAsync(Guid customerId, string storeId)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Where(i => i.CustomerId == customerId && i.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<List<InvoiceDetail>> GetDetailsAsync(string invoiceID, string storeId)
        {
            // Join qua Invoice để kiểm tra invoice có thuộc đúng Store không
            return await _context.InvoiceDetails
                .Include(d => d.Invoice)
                .Where(d => d.InvoiceId == invoiceID && d.Invoice.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByOrderCodeAsync(long orderCode)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Include(i => i.Details).ThenInclude(d => d.Item)
                .Where(i => i.PayOsOrderCode == orderCode)
                .SingleOrDefaultAsync();
        }

    }
}
