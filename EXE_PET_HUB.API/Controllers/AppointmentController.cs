
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("appointments")]
        public async Task<ActionResult<List<AppointmentDto>>> GetAll()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("appointment/{id}")]
        public async Task<ActionResult<AppointmentDto>> GetById(string id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpPost("appointment")]
        public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentDto dto)
        {
            var appointment = await _appointmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPatch("appointment/{id}")]
        public async Task<ActionResult> Update(string id, UpdateAppointmentDto dto)
        {
            var appointment = await _appointmentService.Update(id, dto);
            if (!appointment) return NotFound();
            return NoContent();
        }

        [HttpDelete("appointment/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var appointment = await _appointmentService.Delete(id);
            if (!appointment) return NotFound();
            return NoContent();
        }

    }

}