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
        public async Task<ActionResult> Create(CreateMedicalRecordDto dto)
        {
            var record = await _recordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(string id, UpdateMedicalRecordDto dto)
        {
           var record = await _recordService.Update(id, dto);
            if(!record) return NotFound();
            return NoContent();
        }
    }
}
