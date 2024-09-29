namespace PedagangPulsa.API.Model
{
    public class DuitkuPaymentRequest
    {
        public string MerchantCode { get; set; }
        public int PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string MerchantOrderId { get; set; }
        public string ProductDetails { get; set; }
        public string AdditionalParam { get; set; }
        public string MerchantUserInfo { get; set; }
        public string CustomerVaName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CallbackUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string Signature { get; set; }
        public int ExpiryPeriod { get; set; }
    }

    public class DuitkuInquiryResponse
    {
        public string merchantCode { get; set; }
        public string reference { get; set; }
        public string paymentUrl { get; set; }
        public string qrString { get; set; }
        public string amount { get; set; }
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
    }
    public class DuitkuCallbackRequest
    {
        public string merchantCode { get; set; }
        public int amount { get; set; }
        public string merchantOrderId { get; set; }
        public string productDetail { get; set; }
        public string? additionalParam { get; set; }
        public string paymentCode { get; set; }
        public string resultCode { get; set; }
        public string? merchantUserId { get; set; }
        public string reference { get; set; }
        public string signature { get; set; }
        public string publisherOrderId { get; set; }
        public string spUserHash { get; set; }
        public string settlementDate { get; set; }
        public string issuerCode { get; set; }
    }

}
