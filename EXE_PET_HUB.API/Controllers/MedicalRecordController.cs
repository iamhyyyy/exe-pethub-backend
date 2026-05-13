using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _recordService;
        private readonly IEmailService _emailService;

        public MedicalRecordController(MedicalRecordService recordService, IEmailService emailService)
        {
            _recordService = recordService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetAll()
        {
            var records = await _recordService.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecordDto>> GetById(string id)
        {
            var record = await _recordService.GetByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult> Create(MedicalRecordDto record)
        {
            await _recordService.CreateAsync(record);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(MedicalRecordDto dto)
        {
            await _recordService.Update(dto);
            return NoContent();
        }
    }
}
