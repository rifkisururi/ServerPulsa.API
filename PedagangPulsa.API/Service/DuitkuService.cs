using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PedagangPulsa.API.Service
{
    public class DuitkuService : IDuitkuService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private string _apiKey;
        private string _merchantCode;
        private string _paymentMethod;
        private string _callback;
        private string _returnUrl;
        private string _prefixTrx;

        public DuitkuService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _apiKey         = _configuration["Duitku:apiKey"];
            _merchantCode   = _configuration["Duitku:merchantCode"];
            _paymentMethod  = _configuration["Duitku:paymentMethod"];
            _callback       = _configuration["Duitku:callbackUrl"];
            _returnUrl = _configuration["Duitku:returnUrl"];
            _prefixTrx = _configuration["Duitku:prefix"];
        }

        public async Task<DuitkuInquiryResponse> CreatePaymentRequestAsync(DuitkuPaymentRequest request)
        {
            var apiUrl = "https://sandbox.duitku.com/webapi/api/merchant/v2/inquiry";
            
            // Handle null values by setting defaults
            request.MerchantCode = _merchantCode;
            request.PaymentAmount = request.PaymentAmount; // Default to 20000 if not provided
            request.PaymentMethod = _paymentMethod;
            request.MerchantOrderId = _prefixTrx + request.MerchantOrderId; // Generate a unique order ID if not provided
            request.ProductDetails = request.ProductDetails;
            request.AdditionalParam =  "";
            request.MerchantUserInfo = "";
            request.CustomerVaName = "John Doe";
            request.Email = "pelanggan_anda@email.com";
            request.PhoneNumber = request.PhoneNumber;
            request.CallbackUrl = _callback;
            request.ReturnUrl = _returnUrl;
            request.ExpiryPeriod = request.ExpiryPeriod != 0 ? request.ExpiryPeriod : 15; // Default to 15 if not provided

            request.Signature = generateSignature(request.MerchantCode, request.MerchantOrderId, request.PaymentAmount);

            // Serialize the request body
            var jsonContent = JsonSerializer.Serialize(request);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Send POST request to Duitku API
            var response = await _httpClient.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON result into DuitkuInquiryResponse model
                var result = JsonSerializer.Deserialize<DuitkuInquiryResponse>(jsonResponse);

                return result;
            }

            return null;
        }
        public string generateSignature(string merchantCode, string merchantOrderId, int paymentAmount)
        {
            // MD5(merchantCode + merchantOrderId + paymentAmount + apiKey)
            return GenerateMD5Hash(merchantCode + merchantOrderId + paymentAmount + _apiKey);
        }
        public string generateSignatureCallback(string merchantCode, string merchantOrderId, int paymentAmount)
        {
            //merchantcode + amount + merchantOrderId + apiKey
            return GenerateMD5Hash(merchantCode + paymentAmount + merchantOrderId + _apiKey);
        }

        
            
        private static string GenerateMD5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string.
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
