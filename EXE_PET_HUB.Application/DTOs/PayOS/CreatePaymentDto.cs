namespace EXE_PET_HUB.Application.DTOs.PayOS
{
    public class CreatePaymentDto
    {
        /// <summary>
        /// ID của Invoice cần thanh toán (GUID string)
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// Tên người mua (hiển thị trên trang thanh toán PayOS)
        /// </summary>
        public string? BuyerName { get; set; }

        /// <summary>
        /// Email người mua
        /// </summary>
        public string? BuyerEmail { get; set; }

        /// <summary>
        /// Mô tả đơn hàng — PayOS giới hạn 25 ký tự, không dấu tiếng Việt
        /// </summary>
        public string? Description { get; set; }
    }
}
