using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetRepository _petRepository;
        private readonly IEmailService _emailService;

        public PetsController(IPetRepository petRepository, IEmailService emailService)
        {
            _petRepository = petRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pet>>> GetAll()
        {
            var pets = await _petRepository.GetAllAsync();
            return Ok(pets);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePet(Pet pet)
        {
            // Lưu DB trước
            await _petRepository.AddAsync(pet);

            // Gửi email thông báo (nếu fail thì log nhưng không crash API)
            bool emailSent = false;
            string? emailError = null;
            
            try
            {
                await _emailService.SendEmailAsync(
                    "huyheo05092004@gmail.com",
                    "Pet Created Successfully",
                    $"<h2>Pet {pet.Name} đã được tạo thành công!</h2>");
                emailSent = true;
                Console.WriteLine($"Email sent successfully to huyheo05092004@gmail.com");
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng vẫn trả về success vì pet đã được lưu vào DB
                emailSent = false;
                emailError = ex.Message;
                Console.WriteLine($"Warning: Failed to send email notification: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return Ok(new { 
                message = "Pet created successfully", 
                pet = pet,
                emailSent = emailSent,
                emailError = emailError // Trả về lỗi nếu có để debug
            });
        }

    }

}