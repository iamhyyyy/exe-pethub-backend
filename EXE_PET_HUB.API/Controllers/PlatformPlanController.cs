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

        // Tất cả role đều xem được danh sách gói (chỉ active)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _unitOfWork.PlatformPlanRepository.GetAllActiveAsync();
            return Ok(plans);
        }

        // Xem chi tiết 1 gói (dùng khi manager chọn gói để mua)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(id);
            if (plan == null) return NotFound(new { message = "Không tìm thấy gói." });
            return Ok(plan);
        }

        // Chỉ Admin mới tạo được gói mới
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

        // Chỉ Admin mới cập nhật gói
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

        // Chỉ Admin mới xóa (ẩn) gói — soft delete bằng IsActive = false
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(id);
            if (plan == null) return NotFound(new { message = "Không tìm thấy gói." });

            // Soft delete — ẩn gói thay vì xóa hẳn để không ảnh hưởng lịch sử
            plan.IsActive = false;
            _unitOfWork.PlatformPlanRepository.Update(plan);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
