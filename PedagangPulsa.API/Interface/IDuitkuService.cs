using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Interface
{
    public interface IDuitkuService
    {
        Task<DuitkuInquiryResponse> CreatePaymentRequestAsync(DuitkuPaymentRequest request);
        public string generateSignature(string merchantCode, string merchantOrderId, int paymentAmount);
        public string generateSignatureCallback(string merchantCode, string merchantOrderId, int paymentAmount);
    }
}
