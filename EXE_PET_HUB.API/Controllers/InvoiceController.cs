using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<InvoiceDto>>> GetAll()
        {
            var items = await _invoiceService.GetAllAsync();
            return Ok(items);
        }
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllByCusID(Guid customerId)
        {
            var items = await _invoiceService.GetAllByCusIDAsync(customerId);
            return Ok(items);
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<List<InvoiceDetailsDto>>> GetDetailByInvoiceID(string invoiceId)
        {
            var items = await _invoiceService.GetInvoiceDetailsAsync(invoiceId);
            return Ok(items);
        }

    }
}
