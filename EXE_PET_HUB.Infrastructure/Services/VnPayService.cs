using Microsoft.Extensions.Configuration;
using EXE_PET_HUB.Application.DTOs.VnPay;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Enums;
using EXE_PET_HUB.Infrastructure.Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            //var tick = DateTime.Now.AddHours(7).Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];
            var ipnUrl = _configuration["Vnpay:PaymentIpnUrl"];
            var infoInvoice = await _unitOfWork.InvoiceRepository.GetInvoiceAsync(model.InvoiceId);

            if (infoInvoice == null)
                throw new Exception($"Invoice '{model.InvoiceId}' not found.");

            if (infoInvoice.Status != InvoiceStatus.Pending)
                throw new Exception($"Invoice is already in status '{infoInvoice.Status}', cannot process payment.");

            var Amount = infoInvoice.TotalAmount;

            // VNPay yêu cầu vnp_TxnRef chỉ chứa ký tự alphanumeric — bỏ dấu '-' khỏi GUID
            var txnRef = model.InvoiceId.Replace("-", "");

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {(long)Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_IpnUrl", ipnUrl);
            pay.AddRequestData("vnp_TxnRef", txnRef);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }

        public async Task<bool> ProcessIpnAsync(IQueryCollection collections)
        {
            // Bước 1: Xác thực chữ ký từ VNPAY
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            if (!response.Success)
                return false;

            // Bước 2: Khôi phục dấu '-' vào GUID (vnp_TxnRef được gửi đi không có '-')
            var rawTxnRef = response.OrderId;
            var invoiceId = rawTxnRef.Length == 32
                ? $"{rawTxnRef[..8]}-{rawTxnRef[8..12]}-{rawTxnRef[12..16]}-{rawTxnRef[16..20]}-{rawTxnRef[20..]}"
                : rawTxnRef;

            var invoice = await _unitOfWork.InvoiceRepository.GetInvoiceAsync(invoiceId);

            if (invoice == null)
                return false;

            // Bước 3: Chống duplicate — chỉ xử lý nếu đang ở trạng thái Pending
            if (invoice.Status != InvoiceStatus.Pending)
                return false;

            // Bước 4: Cập nhật trạng thái dựa vào ResponseCode
            if (response.VnPayResponseCode == "00")
                invoice.Status = InvoiceStatus.Paid;
            else
                invoice.Status = InvoiceStatus.Failed;

            // Bước 5: Lưu xuống DB
            _unitOfWork.InvoiceRepository.Update(invoice);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
