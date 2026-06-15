
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]  // Cả Manager lẫn Customer đều dùng được
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        //[HttpGet("appointments")]
        //public async Task<ActionResult<List<AppointmentDto>>> GetAll()
        //{
        //    var appointments = await _appointmentService.GetAllAsync();
        //    return Ok(appointments);
        //}
        

        [HttpGet("appointments/store/{storeId}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetAllByStoreId(string storeId)
        {
            var appointments = await _appointmentService.GetAllAsyncByStoreId(storeId);
            return Ok(appointments);
        }

        //[HttpGet("appointment/{id}")]
        //public async Task<ActionResult<AppointmentDto>> GetById(string id)
        //{
        //    var appointment = await _appointmentService.GetByIdAsync(id);
        //    if (appointment == null) return NotFound();
        //    return Ok(appointment);
        //}

        [HttpGet("appointment/{id}/store/{storeId}")]
        public async Task<ActionResult<AppointmentDto>> GetById(string id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpGet("appointments/pet/{petId}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetByPetId(string petId)
        {
            var appointments = await _appointmentService.GetByPetIdAsync(petId);
            return Ok(appointments);
        }

        [HttpGet("appointments/customer/{customerId}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetByCustomerId(Guid customerId)
        {
            var appointments = await _appointmentService.GetByCustomerIdAsync(customerId);
            return Ok(appointments);
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