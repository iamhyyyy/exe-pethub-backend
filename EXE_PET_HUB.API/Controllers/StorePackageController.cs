using EXE_PET_HUB.Application.DTOs.StorePackage;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorePackageController : ControllerBase
    {
        private readonly IStorePackageService _storePackageService;
        private readonly IPayOsService _payOsService;

        public StorePackageController(IStorePackageService storePackageService, IPayOsService payOsService)
        {
            _storePackageService = storePackageService;
            _payOsService = payOsService;
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create([FromBody] CreateStorePackageDto dto)
        {
            try
            {
                var managerIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(managerIdStr) || !Guid.TryParse(managerIdStr, out var managerId))
            return Unauthorized(new { message = "Không xác định được Manager." });
                var package = await _storePackageService.CreateAsync(managerId, dto);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("payment")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> CreateCheckoutUrl([FromBody] CreateStorePackageCheckoutDto dto)
        {
            try
            {
                var checkoutUrl = await _payOsService.CreateStorePackageCheckoutUrlAsync(dto);
                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("manager/{managerId}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> GetByManagerId(Guid managerId)
        {
            try
            {
                var packages = await _storePackageService.GetByManagerIdAsync(managerId);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var packages = await _storePackageService.GetAllAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var package = await _storePackageService.GetByIdAsync(id);
                if (package == null) return NotFound(new { message = "Không tìm thấy gói." });
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
