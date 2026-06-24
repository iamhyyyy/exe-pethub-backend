using EXE_PET_HUB.Application.DTOs.PayOS;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOsService _payOsService;

        public PaymentController(IPayOsService payOsService)
        {
            _payOsService = payOsService;
        }

        [HttpPost]
        [Authorize(Roles = "manager")] 
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var checkoutUrl = await _payOsService.CreatePaymentLinkAsync(dto);
                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOsWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var jsonBody = await reader.ReadToEndAsync();

            var success = await _payOsService.ProcessWebhookAsync(jsonBody);

            // PayOS yêu cầu luôn trả HTTP 200
            return Ok(new { success });
        }

        [HttpGet("return")]
        [AllowAnonymous]
        public IActionResult PaymentReturn(
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] string status,
            [FromQuery] long orderCode,
            [FromQuery] string cancel)
        {
            if (code == "00" && status == "PAID")
                return Ok(new
                {
                    status = "success",
                    message = "Thanh toán thành công!",
                    orderCode,
                    paymentId = id
                });

            if (cancel == "true")
                return Ok(new
                {
                    status = "cancelled",
                    message = "Người dùng đã hủy thanh toán.",
                    orderCode
                });

            return Ok(new
            {
                status = "failed",
                message = $"Thanh toán thất bại. Mã lỗi: {code}",
                orderCode
            });
        }


        [HttpGet("info/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentInfo(long orderCode)
        {
            try
            {
                var info = await _payOsService.GetPaymentInfoAsync(orderCode);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{orderCode}")]
        [Authorize]
        public async Task<IActionResult> CancelPayment(long orderCode)
        {
            try
            {
                var result = await _payOsService.CancelPaymentAsync(orderCode);
                return Ok(new { message = "Đã hủy payment link thành công.", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
