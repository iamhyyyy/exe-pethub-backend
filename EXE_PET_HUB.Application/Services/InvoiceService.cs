using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Services
{
    public class InvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var items = await _unitOfWork.InvoiceRepository.GetAllInvoicesWithDetailsAsync();
            return items == null ? null : _mapper.Map<List<InvoiceDto>>(items);
        }

        public async Task<InvoiceDto?> GetByIdAsync(string id)
        {
            var items = await _unitOfWork.InvoiceRepository.GetInvoiceWithDetailsAsync(id);
            return items == null ? null : _mapper.Map<InvoiceDto>(items);
        }

        public async Task<List<InvoiceDto>> GetAllByCusIDAsync(Guid cusID)
        {
            var items = await _unitOfWork.InvoiceRepository.GetAllInvoicesDetailsByCusIDAsync(cusID);
            return items == null ? null : _mapper.Map<List<InvoiceDto>>(items);
        }

        public async Task<List<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceID)
        {
            var items = await _unitOfWork.InvoiceRepository.GetDetailsAsync(invoiceID);
            return items == null ? null : _mapper.Map<List<InvoiceDetailsDto>>(items);
        }

        public async Task<ResponseInvoiceOfCreateDto> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            if (dto.Details == null || !dto.Details.Any())
            {
                throw new Exception("Invoice must have at least one detail");
            }

            var invoice = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                PetId = dto.PetId,
                AppointmentId = dto.AppointmentId,
                CustomerId = dto.CustomerId,
                CreatedAt = DateTime.UtcNow,

                Details = dto.Details.Select(x => new InvoiceDetail
                {
                    ItemName = x.ItemName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Subtotal = x.Price * x.Quantity
                }).ToList(),

                TotalAmount = dto.Details.Sum(x => x.Price * x.Quantity)
            };
            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ResponseInvoiceOfCreateDto>(invoice);
        }
    }
}
