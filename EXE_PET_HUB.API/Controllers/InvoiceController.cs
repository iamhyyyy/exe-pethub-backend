using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<InvoiceDto>>> GetAll()
        {
            var items = await _invoiceService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<InvoiceDto>> GetById(string id)
        {
            var item = await _invoiceService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllByCusID(Guid customerId)
        {
            var items = await _invoiceService.GetAllByCusIDAsync(customerId);
            return Ok(items);
        }

        [HttpGet("invoice-detail/{invoiceId}")]
        [Authorize]
        public async Task<ActionResult<List<InvoiceDetailsDto>>> GetDetailByInvoiceID(string invoiceId)
        {
            var items = await _invoiceService.GetInvoiceDetailsAsync(invoiceId);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<ResponseInvoiceOfCreateDto>> Create(CreateInvoiceDto dto)
        {
            var createdInvoice = await _invoiceService.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
        }

        [HttpPatch("confirm-transaction/{invoiceid}")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> MarkAsPaid(string invoiceid)
        {
            try
            {
                var result = await _invoiceService.MarkAsPaidAsync(invoiceid);
                return Ok(new { message = "Invoice marked as Paid successfully", invoiceId = invoiceid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
