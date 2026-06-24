using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformPlanController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlatformPlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _unitOfWork.PlatformPlanRepository.GetAllActiveAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(id);
            if (plan == null) return NotFound(new { message = "Không tìm thấy gói." });
            return Ok(plan);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] PlatformPlan dto)
        {
            var plan = new PlatformPlan
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Price = dto.Price,
                DurationInDays = dto.DurationInDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };
            await _unitOfWork.PlatformPlanRepository.AddAsync(plan);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(GetById), new { id = plan.Id }, plan);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(string id, [FromBody] PlatformPlan dto)
        {
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(id);
            if (plan == null) return NotFound(new { message = "Không tìm thấy gói." });

            plan.Name = dto.Name;
            plan.Price = dto.Price;
            plan.DurationInDays = dto.DurationInDays;
            plan.IsActive = dto.IsActive;

            _unitOfWork.PlatformPlanRepository.Update(plan);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(id);
            if (plan == null) return NotFound(new { message = "Không tìm thấy gói." });

            plan.IsActive = false;
            _unitOfWork.PlatformPlanRepository.Update(plan);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
