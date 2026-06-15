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

        /// <summary>
        /// API 1: Tạo payment link PayOS.
        /// FE gọi → nhận checkoutUrl → redirect user sang trang thanh toán PayOS.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "manager")]  // Chỉ Manager tạo link thanh toán
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

        /// <summary>
        /// API 2: Webhook từ PayOS (server-to-server).
        /// PayOS gọi endpoint này sau khi có kết quả thanh toán.
        /// Đây là nơi DUY NHẤT cập nhật trạng thái Invoice trong DB.
        /// PayOS yêu cầu response HTTP 200 — nếu không sẽ retry nhiều lần.
        /// </summary>
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

        /// <summary>
        /// API 3: ReturnURL — PayOS redirect browser user về đây sau khi thanh toán.
        /// Đây chỉ để hiển thị kết quả cho user, KHÔNG cập nhật DB (DB đã được cập nhật qua webhook).
        /// Khi FE có trang kết quả riêng, đổi PayOS:ReturnUrl trong appsettings sang URL của FE.
        /// </summary>
        [HttpGet("return")]
        [AllowAnonymous]
        public IActionResult PaymentReturn(
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] string status,
            [FromQuery] long orderCode,
            [FromQuery] string cancel)
        {
            // PayOS trả code "00" là thành công
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

        /// <summary>
        /// API 4 (Bonus): Query trạng thái giao dịch từ PayOS theo orderCode.
        /// Dùng để check thủ công khi cần debug.
        /// </summary>
        [HttpGet("info/{orderCode}")]
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

        /// <summary>
        /// API 5 (Bonus): Hủy payment link theo orderCode.
        /// </summary>
        [HttpDelete("{orderCode}")]
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
