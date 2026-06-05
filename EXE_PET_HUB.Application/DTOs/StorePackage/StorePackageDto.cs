using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.DTOs.StorePackage
{
    /// <summary>
    /// DTO trả về thông tin gói đã mua
    /// </summary>
    public class StorePackagePaymentDto
    {
        public string Id { get; set; } = null!;
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; } = null!;
        public string PackageType { get; set; } = null!;
        public double Price { get; set; }
        public int DurationInDays { get; set; }
        public long? PayOsOrderCode { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionNo { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO để Manager tạo đơn mua gói
    /// </summary>
    public class CreateStorePackageDto
    {
        /// <summary>
        /// Guid của Manager mua gói
        /// </summary>
        public Guid ManagerId { get; set; }

        /// <summary>
        /// Id của gói (lấy từ bảng Item với Type = Plan)
        /// </summary>
        public string ItemId { get; set; } = null!;
    }

    /// <summary>
    /// DTO để tạo PayOS payment link cho gói
    /// </summary>
    public class CreateStorePackageCheckoutDto
    {
        public string PackageId { get; set; } = null!;
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
    }
}
