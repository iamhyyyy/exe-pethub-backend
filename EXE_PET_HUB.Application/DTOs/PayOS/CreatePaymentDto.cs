namespace EXE_PET_HUB.Application.DTOs.PayOS
{
    public class CreatePaymentDto
    {

        public string InvoiceId { get; set; }
        public string StoreId { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? Description { get; set; }
    }
}
