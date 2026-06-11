namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IPayOsService
    {
        /// <summary>
        /// Tạo payment link PayOS — trả về checkoutUrl để redirect user sang trang thanh toán
        /// </summary>
        Task<string> CreatePaymentLinkAsync(Application.DTOs.PayOS.CreatePaymentDto dto);

        /// <summary>
        /// Xử lý webhook từ PayOS (server-to-server) — verify chữ ký, update Invoice status trong DB.
        /// Nhận body dạng dictionary để Application layer không phụ thuộc vào PayOS SDK.
        /// </summary>
        Task<bool> ProcessWebhookAsync(string jsonBody);

        /// <summary>
        /// Query trạng thái giao dịch từ PayOS theo orderCode — trả về string JSON để đơn giản
        /// </summary>
        Task<object> GetPaymentInfoAsync(long orderCode);

        /// <summary>
        /// Hủy payment link theo orderCode
        /// </summary>
        Task<object> CancelPaymentAsync(long orderCode);

        /// <summary>
        /// Tạo payment link PayOS cho Manager mua gói Premium
        /// </summary>
        Task<string> CreateStorePackageCheckoutUrlAsync(DTOs.StorePackage.CreateStorePackageCheckoutDto dto);
    }
}
