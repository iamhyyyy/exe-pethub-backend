using EXE_PET_HUB.Application.DTOs.StorePackage;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create([FromBody] CreateStorePackageDto dto)
        {
            try
            {
                var package = await _storePackageService.CreateAsync(dto);
                return Ok(package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/checkout")]
        public async Task<IActionResult> CreateCheckoutUrl(string id, [FromBody] CreateStorePackageCheckoutDto dto)
        {
            try
            {
                if (id != dto.PackageId)
                {
                    return BadRequest(new { message = "PackageId in path and body do not match." });
                }

                var checkoutUrl = await _payOsService.CreateStorePackageCheckoutUrlAsync(dto);
                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("manager/{managerId}")]
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

        [HttpGet("{id}")]
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
