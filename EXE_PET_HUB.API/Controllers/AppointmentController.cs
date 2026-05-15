
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAll()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetById(string id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentDto dto)
        {
            var appointment = await _appointmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(string id, UpdateAppointmentDto dto)
        {
            var appointment = await _appointmentService.Update(id, dto);
            if (!appointment) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]

        //[HttpGet("test")]
        //public async Task<IActionResult> SendTest()
        //{
        //    await _emailService.SendEmailAsync(
        //        "huyndse184016@fpt.edu.vn",
        //        "Test Mail",
        //        "Hello from PetHub 🐶"
        //    );

        //    return Ok("Email sent");
        //}
    }

}