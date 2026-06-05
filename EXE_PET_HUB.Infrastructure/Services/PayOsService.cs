using EXE_PET_HUB.Application.DTOs.PayOS;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Enums;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System.Text.Json;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class PayOsService : IPayOsService
    {
        private readonly PayOSClient _payOS;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public PayOsService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;

            _payOS = new PayOSClient(
                clientId:    configuration["PayOS:ClientId"]!,
                apiKey:      configuration["PayOS:ApiKey"]!,
                checksumKey: configuration["PayOS:ChecksumKey"]!
            );
        }

        // ─── API 1: Tạo payment link ────────────────────────────────────────────────
        public async Task<string> CreatePaymentLinkAsync(CreatePaymentDto dto)
        {
            // 1. Kiểm tra Invoice tồn tại và đang ở trạng thái Pending
            var invoice = await _unitOfWork.InvoiceRepository.GetInvoiceAsync(dto.InvoiceId);

            if (invoice == null)
                throw new Exception($"Invoice '{dto.InvoiceId}' không tìm thấy.");

            if (invoice.Status != InvoiceStatus.Pending)
                throw new Exception($"Invoice đang ở trạng thái '{invoice.Status}', không thể tạo thanh toán.");

            // 2. Tạo orderCode duy nhất (long) = timestamp ms * 10 + random digit
            var random = new Random();
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 10 + random.Next(0, 10);

            // 3. Lưu orderCode vào Invoice để tra cứu khi nhận webhook
            invoice.PayOsOrderCode = orderCode;
            _unitOfWork.InvoiceRepository.Update(invoice);
            await _unitOfWork.CompleteAsync();

            // 4. Build description — PayOS giới hạn 25 ký tự, không dấu tiếng Việt
            var description = RemoveDiacritics(dto.Description ?? "Thanh toan PetHub");
            if (description.Length > 25) description = description[..25];

            var amount = (int)invoice.TotalAmount;

            // 5. Build request theo PayOS SDK v2
            var request = new CreatePaymentLinkRequest
            {
                OrderCode   = orderCode,
                Amount      = amount,
                Description = description,
                CancelUrl   = _configuration["PayOS:CancelUrl"]!,
                ReturnUrl   = _configuration["PayOS:ReturnUrl"]!,
                BuyerName   = dto.BuyerName,
                BuyerEmail  = dto.BuyerEmail,
                Items       = new List<PaymentLinkItem>
                {
                    new PaymentLinkItem
                    {
                        Name     = "Dich vu thu cung",
                        Quantity = 1,
                        Price    = amount
                    }
                }
            };

            // 6. Gọi PayOS API → nhận checkoutUrl
            var result = await _payOS.PaymentRequests.CreateAsync(request);
            return result.CheckoutUrl;
        }

        // ─── API 2: Xử lý webhook (server-to-server từ PayOS) ───────────────────────
        public async Task<bool> ProcessWebhookAsync(string jsonBody)
        {
            try
            {
                // Bước 1: Deserialize JSON body thành Webhook object theo PayOS SDK v2
                var webhookPayload = JsonSerializer.Deserialize<Webhook>(jsonBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (webhookPayload == null) return false;

                // Bước 2: PayOS SDK tự verify chữ ký HMAC
                var data = await _payOS.Webhooks.VerifyAsync(webhookPayload);

                if (data == null) return false;

                // Bước 3: Tìm Invoice theo orderCode đã lưu
                var invoice = await _unitOfWork.InvoiceRepository.GetInvoiceByOrderCodeAsync(data.OrderCode);

                if (invoice == null) return false;

                // Bước 4: Idempotency — chỉ xử lý nếu đang Pending
                if (invoice.Status != InvoiceStatus.Pending) return false;

                // Bước 5: Cập nhật trạng thái — PayOS code "00" = thành công
                invoice.Status = data.Code == "00"
                    ? InvoiceStatus.Paid
                    : InvoiceStatus.Failed;

                // Bước 6: Lưu DB
                _unitOfWork.InvoiceRepository.Update(invoice);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        // ─── API 3: Query trạng thái giao dịch ──────────────────────────────────────
        public async Task<object> GetPaymentInfoAsync(long orderCode)
        {
            return await _payOS.PaymentRequests.GetAsync(orderCode);
        }

        // ─── API 4: Hủy payment link ─────────────────────────────────────────────────
        public async Task<object> CancelPaymentAsync(long orderCode)
        {
            return await _payOS.PaymentRequests.CancelAsync(orderCode);
        }

        // ─── Helper: Xóa dấu tiếng Việt ─────────────────────────────────────────────
        private static string RemoveDiacritics(string text)
        {
            var map = new Dictionary<string, string>
            {
                {"à","a"},{"á","a"},{"â","a"},{"ã","a"},{"ả","a"},{"ạ","a"},
                {"ă","a"},{"ắ","a"},{"ằ","a"},{"ẳ","a"},{"ẵ","a"},{"ặ","a"},
                {"ấ","a"},{"ầ","a"},{"ẩ","a"},{"ẫ","a"},{"ậ","a"},
                {"è","e"},{"é","e"},{"ê","e"},{"ẹ","e"},{"ẻ","e"},{"ẽ","e"},
                {"ế","e"},{"ề","e"},{"ể","e"},{"ễ","e"},{"ệ","e"},
                {"ì","i"},{"í","i"},{"ỉ","i"},{"ĩ","i"},{"ị","i"},
                {"ò","o"},{"ó","o"},{"ô","o"},{"õ","o"},{"ỏ","o"},{"ọ","o"},
                {"ố","o"},{"ồ","o"},{"ổ","o"},{"ỗ","o"},{"ộ","o"},
                {"ơ","o"},{"ớ","o"},{"ờ","o"},{"ở","o"},{"ỡ","o"},{"ợ","o"},
                {"ù","u"},{"ú","u"},{"û","u"},{"ủ","u"},{"ũ","u"},{"ụ","u"},
                {"ư","u"},{"ứ","u"},{"ừ","u"},{"ử","u"},{"ữ","u"},{"ự","u"},
                {"ỳ","y"},{"ý","y"},{"ỷ","y"},{"ỹ","y"},{"ỵ","y"},{"đ","d"},
                {"À","A"},{"Á","A"},{"Â","A"},{"Ã","A"},{"Ả","A"},{"Ạ","A"},
                {"Ă","A"},{"Ắ","A"},{"Ằ","A"},{"Ẳ","A"},{"Ẵ","A"},{"Ặ","A"},
                {"È","E"},{"É","E"},{"Ê","E"},{"Ẹ","E"},{"Ẻ","E"},{"Ẽ","E"},
                {"Ế","E"},{"Ề","E"},{"Ể","E"},{"Ễ","E"},{"Ệ","E"},
                {"Ì","I"},{"Í","I"},{"Ỉ","I"},{"Ĩ","I"},{"Ị","I"},
                {"Ò","O"},{"Ó","O"},{"Ô","O"},{"Õ","O"},{"Ỏ","O"},{"Ọ","O"},
                {"Ố","O"},{"Ồ","O"},{"Ổ","O"},{"Ỗ","O"},{"Ộ","O"},
                {"Ơ","O"},{"Ớ","O"},{"Ờ","O"},{"Ở","O"},{"Ỡ","O"},{"Ợ","O"},
                {"Ù","U"},{"Ú","U"},{"Û","U"},{"Ủ","U"},{"Ũ","U"},{"Ụ","U"},
                {"Ư","U"},{"Ứ","U"},{"Ừ","U"},{"Ử","U"},{"Ữ","U"},{"Ự","U"},
                {"Ỳ","Y"},{"Ý","Y"},{"Ỷ","Y"},{"Ỹ","Y"},{"Ỵ","Y"},{"Đ","D"}
            };
            foreach (var kv in map)
                text = text.Replace(kv.Key, kv.Value);
            return text;
        }
    }
}
