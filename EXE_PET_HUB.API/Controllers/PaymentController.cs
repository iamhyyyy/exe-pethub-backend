using EXE_PET_HUB.Application.DTOs.VnPay;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            try
            {
                var url = await _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Ok(new
                {
                    paymentUrl = url
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint tạm thời làm ReturnUrl khi FE chưa sẵn sàng.
        /// VNPay sẽ redirect người dùng về đây sau khi thanh toán.
        /// Set PaymentBackReturnUrl = "http://localhost:7000/api/payment/return" trong appsettings.
        /// </summary>
        [HttpGet("return")]
        [AllowAnonymous]
        public IActionResult PaymentReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success)
                return Ok(new
                {
                    status = "failed",
                    message = "Xác thực chữ ký thất bại hoặc giao dịch không hợp lệ.",
                    data = response
                });

            if (response.VnPayResponseCode == "00")
                return Ok(new
                {
                    status = "success",
                    message = "Thanh toán thành công!",
                    data = response
                });

            return Ok(new
            {
                status = "cancelled",
                message = $"Giao dịch thất bại. Mã lỗi VNPay: {response.VnPayResponseCode}",
                data = response
            });
        }

        [HttpGet("ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> IpnCallback()
        {
            var success = await _vnPayService.ProcessIpnAsync(Request.Query);

            // VNPAY bắt buộc phải return HTTP 200 với đúng format này
            // Nếu trả 4xx/5xx, VNPAY sẽ retry nhiều lần
            if (success)
                return Ok(new { RspCode = "00", Message = "Confirm Success" });

            return Ok(new { RspCode = "99", Message = "Fail" });
        }
    }
}
