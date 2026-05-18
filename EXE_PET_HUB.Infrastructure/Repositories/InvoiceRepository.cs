using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceAsync(string invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Where(i => i.Id == invoiceId)
                .SingleOrDefaultAsync();
        }
        public async Task<List<Invoice>> GetAllInvoicesByCusIDAsync(Guid customerId)
        {
            return await _context.Invoices
                .Include(i => i.Pet)
                .Include(i => i.Appointment)
                .Include(i => i.Customer)
                .Where(i => i.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<List<InvoiceDetail>> GetDetailsAsync(string invoiceID)
        {
            return await _context.InvoiceDetails
                .Where(i => i.InvoiceId == invoiceID)
                .ToListAsync();
        }

    }
}
