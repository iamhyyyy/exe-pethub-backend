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
            return _mapper.Map<List<InvoiceDto>>(items);
        }

        public async Task<List<InvoiceDto>> GetAllByCusIDAsync(Guid cusID)
        {
            var items = await _unitOfWork.InvoiceRepository.GetAllInvoicesDetailsByCusIDAsync(cusID);
            return _mapper.Map<List<InvoiceDto>>(items);
        }

        public async Task<List<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceID)
        {
            var items = await _unitOfWork.InvoiceRepository.GetDetailsAsync(invoiceID);
            return _mapper.Map<List<InvoiceDetailsDto>>(items);
        }
    }
}
