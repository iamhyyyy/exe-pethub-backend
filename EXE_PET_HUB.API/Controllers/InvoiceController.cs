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
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<InvoiceDto>>> GetAll()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var items = await _invoiceService.GetAllAsync(id);
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<InvoiceDto>> GetById(string id)
        {
            var stoid = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var item = await _invoiceService.GetByIdAsync(id, stoid);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllByCusID(Guid customerId)
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var items = await _invoiceService.GetAllByCusIDAsync(customerId, id);
            return Ok(items);
        }

        [HttpGet("invoice-detail/{invoiceId}")]
        [Authorize]
        public async Task<ActionResult<List<InvoiceDetailsDto>>> GetDetailByInvoiceID(string invoiceId)
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            if (string.IsNullOrEmpty(storeId))
                return Unauthorized(new { message = "StoreId not found in token." });
            var items = await _invoiceService.GetInvoiceDetailsAsync(invoiceId, storeId);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<ResponseInvoiceOfCreateDto>> Create(CreateInvoiceDto dto)
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var createdInvoice = await _invoiceService.CreateInvoiceAsync(dto, id);
            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, createdInvoice);
        }

        [HttpPatch("confirm-transaction/{invoiceid}")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> MarkAsPaid(string invoiceid)
        {
            try
            {
                var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
                var result = await _invoiceService.MarkAsPaidAsync(invoiceid, id);
                return Ok(new { message = "Invoice marked as Paid successfully", invoiceId = invoiceid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
