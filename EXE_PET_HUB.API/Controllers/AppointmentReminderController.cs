
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AppointmentReminderController : ControllerBase
    {
        private readonly AppointmentReminderService _appointmentReminderService;
        private readonly IEmailService _emailService;

        public AppointmentReminderController(AppointmentReminderService appointmentReminderService, IEmailService emailService)
        {
            _appointmentReminderService = appointmentReminderService;
            _emailService = emailService;
        }

        //[HttpGet("reminds")]
        //public async Task<ActionResult<List<AppointmentReminderDto>>> GetAll()
        //{
        //    var reminds = await _appointmentReminderService.GetAllAsync();
        //    return Ok(reminds);
        //}

        [HttpGet("remind/{id}")]
        public async Task<ActionResult<AppointmentReminderDto>> GetById(string id)
        {
            var remind = await _appointmentReminderService.GetByIdAsync(id);
            if (remind == null) return NotFound();
            return Ok(remind);
        }

        [HttpGet("remind/appointment/{appointmentId}")]
        public async Task<ActionResult<AppointmentReminderDto>> GetByAppointmentId(string appointmentId)
        {
            var remind = await _appointmentReminderService.GetByAppointmentIdAsync(appointmentId);
            if (remind == null) return NotFound();
            return Ok(remind);
        }

        [HttpPost("remind")]
        public async Task<ActionResult<AppointmentReminderDto>> Create(CreateAppointmentReminderDto dto)
        {
            var remind = await _appointmentReminderService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = remind.Id }, remind);
        }

        [HttpPatch("remind/{id}")]
        public async Task<ActionResult> Update(string id, UpdateAppointmentReminderDto dto)
        {
            var remind = await _appointmentReminderService.Update(id, dto);
            if (!remind) return NotFound();
            return NoContent();
        }

        [HttpDelete("remind/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var remind = await _appointmentReminderService.Delete(id);
            if (!remind) return NotFound();
            return NoContent();
        }
    }
}