using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly MedicalRecordService _recordService;

        public MedicalRecordController(MedicalRecordService recordService)
        {
            _recordService = recordService;
        }

        //[HttpGet("medical_records")]
        //[Authorize(Roles = "manager")]
        //public async Task<ActionResult<List<MedicalRecordDto>>> GetAll()
        //{
        //    var records = await _recordService.GetAllAsync();
        //    return Ok(records);
        //}

        [HttpGet("medical_record/{id}")]
        [Authorize]
        public async Task<ActionResult<MedicalRecordDto>> GetById(string id)
        {
            var record = await _recordService.GetByIdAsync(id);
            if (record == null) return NotFound();
            return Ok(record);
        }

        [HttpGet("medical_records/pet/{petId}")]
        [Authorize]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetByPetId(string petId)
        {
            var records = await _recordService.GetByPetIdAsync(petId);
            return Ok(records);
        }

        [HttpGet("medical_records/appointment/{appointmentId}")]
        [Authorize]
        public async Task<ActionResult<List<MedicalRecordDto>>> GetByAppointmentId(string appointmentId)
        {
            var records = await _recordService.GetByAppointmentIdAsync(appointmentId);
            return Ok(records);
        }

        [HttpPost("medical_record")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Create(CreateMedicalRecordDto dto)
        {
            var record = await _recordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }

        [HttpPatch("medical_record/{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update(string id, UpdateMedicalRecordDto dto)
        {
           var record = await _recordService.Update(id, dto);
            if(!record) return NotFound();
            return NoContent();
        }
    }
}
