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
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var items = await _unitOfWork.InvoiceRepository.GetAllInvoicesAsync();
            return items == null ? null : _mapper.Map<List<InvoiceDto>>(items);
        }

        public async Task<InvoiceDto?> GetByIdAsync(string id)
        {
            var items = await _unitOfWork.InvoiceRepository.GetInvoiceAsync(id);
            return items == null ? null : _mapper.Map<InvoiceDto>(items);
        }

        public async Task<List<InvoiceDto>> GetAllByCusIDAsync(Guid cusID)
        {
            var items = await _unitOfWork.InvoiceRepository.GetAllInvoicesByCusIDAsync(cusID);
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

            // Normalize empty strings/Guid.Empty to null
            var petId = string.IsNullOrWhiteSpace(dto.PetId) ? null : dto.PetId;
            var appointmentId = string.IsNullOrWhiteSpace(dto.AppointmentId) ? null : dto.AppointmentId;
            var customerId = (dto.CustomerId.HasValue && dto.CustomerId.Value != Guid.Empty) ? dto.CustomerId : null;

            // Validate Foreign Keys
            if (petId != null)
            {
                var pet = await _unitOfWork.PetRepository.GetByIdAsync(petId);
                if (pet == null) throw new Exception("Invalid PetId");
            }

            if (appointmentId != null)
            {
                var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointmentId);
                if (appointment == null) throw new Exception("Invalid AppointmentId");
            }

            User? customer = null;
            if (customerId.HasValue)
            {
                var customers = await _unitOfWork.Repository<User>().FindAsync(u => u.Id == customerId.Value);
                customer = customers.FirstOrDefault();
                if (customer == null) 
                {
                    throw new Exception("Invalid CustomerId"); 
                }
            }

            // Fetch Items to get their real prices and names
            var itemIds = dto.Details.Select(d => d.ItemId).Distinct().ToList();
            var items = await _unitOfWork.ItemRepository.FindAsync(i => itemIds.Contains(i.Id));

            if (items.Count != itemIds.Count)
            {
                throw new Exception("One or more ItemIds are invalid");
            }

            var itemDict = items.ToDictionary(i => i.Id);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid().ToString(),
                PetId = petId,
                AppointmentId = appointmentId,
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                Details = new List<InvoiceDetail>()
            };

            decimal totalAmount = 0;

            foreach (var detailDto in dto.Details)
            {
                var item = itemDict[detailDto.ItemId];
                var subtotal = item.Price * detailDto.Quantity;
                
                invoice.Details.Add(new InvoiceDetail
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Price = item.Price,
                    Quantity = detailDto.Quantity,
                    Subtotal = subtotal
                });

                totalAmount += subtotal;
            }

            invoice.TotalAmount = totalAmount;

            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
            await _unitOfWork.CompleteAsync();

            // Fetch the fully populated invoice to map navigation properties correctly
            var savedInvoice = await _unitOfWork.InvoiceRepository.GetInvoiceAsync(invoice.Id);

            // Send Email if Customer exists and has email
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                var customerName = customer.FirstName != null && customer.LastName != null 
                    ? $"{customer.FirstName} {customer.LastName}" 
                    : customer.UserName;

                var sb = new StringBuilder();
                sb.AppendLine($@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 15px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
    <div style='background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); color: white; padding: 30px; text-align: center;'>
        <h1 style='margin: 0; font-size: 24px;'>🐾 PET HUB</h1>
        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Your Pet's Health, Our Priority</p>
    </div>

    <div style='padding: 30px;'>
        <h2 style='color: #2E7D32; margin-top: 0;'>Hello {customerName},</h2>
        <p style='font-size: 16px;'>Thank you for choosing Pet Hub! Here are the details of your recent transaction.</p>
        
        <div style='background-color: #f8fbf8; padding: 20px; border-left: 4px solid #4CAF50; border-radius: 8px; margin: 25px 0;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Date:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{invoice.CreatedAt:dddd, MMM dd, yyyy}</td>
                </tr>
            </table>

            <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                <thead>
                    <tr>
                        <th style='padding: 10px 0; border-bottom: 1px solid #ddd; text-align: left; color: #666;'>Item</th>
                        <th style='padding: 10px 0; border-bottom: 1px solid #ddd; text-align: center; color: #666;'>Qty</th>
                        <th style='padding: 10px 0; border-bottom: 1px solid #ddd; text-align: right; color: #666;'>Subtotal</th>
                    </tr>
                </thead>
                <tbody>");

                foreach (var detail in invoice.Details)
                {
                    sb.AppendLine($@"
                    <tr>
                        <td style='padding: 10px 0; border-bottom: 1px solid #eee;'>{detail.ItemName}</td>
                        <td style='padding: 10px 0; border-bottom: 1px solid #eee; text-align: center;'>{detail.Quantity}</td>
                        <td style='padding: 10px 0; border-bottom: 1px solid #eee; text-align: right;'>{detail.Subtotal:N0} đ</td>
                    </tr>");
                }

                sb.AppendLine($@"
                </tbody>
            </table>

            <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                <tr>
                    <td style='padding: 20px 0 8px 0; border-top: 1px solid #eee;'><strong>Total Amount:</strong></td>
                    <td style='padding: 20px 0 8px 0; text-align: right; border-top: 1px solid #eee;'>
                        <span style='background-color: #007bff; color: white; padding: 5px 15px; border-radius: 20px; font-size: 14px; font-weight: bold;'>
                            {invoice.TotalAmount:N0} VNĐ
                        </span>
                    </td>
                </tr>
            </table>
        </div>

        <p style='text-align: center; font-style: italic; color: #666;'>How was your experience? We'd love to hear your feedback!</p>

        <div style='text-align: center; margin-top: 30px;'>
            <a href='https://pethub.com' style='background-color: #4CAF50; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Visit Our Website</a>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; color: #999;'>
        <p>You received this email because you made a transaction at Pet Hub.</p>
        <p><strong>Pet Hub System</strong><br>Dĩ An, Bình Dương, Vietnam | Hotline: 1900-PET-HUB</p>
    </div>
</div>");

                // Ignore errors from email sending so it doesn't break the invoice creation
                try
                {
                    await _emailService.SendEmailAsync(customer.Email, "Hóa đơn mua hàng từ Pet Hub", sb.ToString());
                }
                catch
                {
                    // Optionally log the exception here
                }
            }

            return _mapper.Map<ResponseInvoiceOfCreateDto>(savedInvoice);
        }
    }
}
