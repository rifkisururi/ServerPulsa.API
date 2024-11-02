using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PedagangPulsa.Web.Controllers
{
    public class TransaksiController : BaseController
    {
        private readonly HttpClient _httpClient;
        private string _baseUrlApi = "https://localhost:7270/api/"; 
        public TransaksiController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrlApi = configuration.GetSection("BaseUrlApi").Value;
        }

        [HttpGet]
        public async Task<IActionResult> Kategori(string katergory)
        {
            string apiUrl = _baseUrlApi+"Transaksi/operator-by-kategory/" + katergory;
            List<OperatorDto> operators = new List<OperatorDto>();

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse>(jsonString);

                    if (apiResult != null && apiResult.status == "Success")
                    {
                        operators = apiResult.data;
                    }
                }
            }
            catch
            {
                // Log error or handle accordingly
            }

            return View(operators);
        }

        [HttpGet]
        public async Task<IActionResult> DetailProduk(int operatorId)
        {
            string apiUrl = _baseUrlApi + $"Transaksi/produk-by-operator/{operatorId}";
            List<ProdukDto> products = new List<ProdukDto>();

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiProductResponse>(jsonString);

                    if (apiResult != null && apiResult.status == "Success")
                    {
                        products = apiResult.data;
                        return Ok(new
                        {
                            status = "Success",
                            data = products
                        });
                    }
                }
            }
            catch
            {
                // Log error or handle accordingly
                
            }
            return BadRequest(new
            {
                status = "Gagal",
                message = "Terjadi kesalahan"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Untuk CSRF protection
        public async Task<IActionResult>  Payment([FromBody] PaymentRequest paymentRequest)
        {
            if (paymentRequest == null || string.IsNullOrEmpty(paymentRequest.no_tujuan))
            {
                return Json(new { success = false, message = "Nomor tujuan tidak valid." });
            }

            // Contoh logika pemrosesan pembayaran
            int paymentId = await ProcessPayment(paymentRequest);

            if (paymentId != 0)
            {
                return Json(new { success = true, trx_id = paymentId });
            }
            else
            {
                return Json(new { success = false, message = "Gagal memproses pembayaran." });
            }
        }

        // Method untuk memproses pembayaran
        private async Task<int> ProcessPayment(PaymentRequest paymentRequest)
        {
            string apiUrl = _baseUrlApi + "Transaksi/draf-order";
            try
            {
                // Buat body request dengan JSON
                var jsonBody = JsonConvert.SerializeObject(paymentRequest);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Kirim POST request ke API eksternal
                var response = await _httpClient.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    // Baca dan parse hasil response
                    var responseString = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiOrderResponse>(responseString);

                    if (apiResult != null && apiResult.status == "Success")
                    {
                        // Pembayaran berhasil diproses
                        return apiResult.trx_id;
                    }
                }
            }
            catch
            {
                // Log error atau tangani sesuai kebutuhan
            }

            // Jika ada masalah dalam proses pembayaran
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> History(int id)
        {
            string apiUrl = _baseUrlApi + $"Transaksi/get-draf-order/{id}";
            TransaksiDetailResponse transaksiDetail = new TransaksiDetailResponse();

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    transaksiDetail.payment = new Payment();
                    transaksiDetail = JsonConvert.DeserializeObject<TransaksiDetailResponse>(jsonString);
                    if(transaksiDetail.payment == null)
                    {
                        transaksiDetail.payment = new Payment();
                    }
                }
            }
            catch
            {
                // Log error atau tangani kesalahan sesuai kebutuhan
            }

            return View(transaksiDetail);
        }
    }

    public class TransaksiDetailResponse
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public Data? data { get; set; }
        public Payment? payment { get; set; }
    }
    public class PaymentRequestDto
    {
        public int Operator_Id { get; set; }
        public string Kode { get; set; }
        public string NoTujuan { get; set; }
    }
    public class ApiResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public List<OperatorDto> data { get; set; }
    }

    public class OperatorDto
    {
        public int operator_Id { get; set; }
        public string provider { get; set; }
        public int jml_Active { get; set; }
    }
    public class ApiProductResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public int count { get; set; }
        public List<ProdukDto> data { get; set; }
    }

    public class ProdukDto
    {
        public int operator_Id { get; set; }
        public string kode { get; set; }
        public string nama_Produk { get; set; }
        public string deskripsi_produk { get; set; }
        public decimal harga { get; set; }
    }

    // DTO Classes for Payment
    public class PaymentRequest
    {
        public int operator_id { get; set; }
        public string kode { get; set; }
        public string no_tujuan { get; set; }
    }

    // DTO Class for Response
    public class ApiOrderResponse
    {
        public string status { get; set; }
        public int trx_id { get; set; }
        
    }

    public class Data
    {
        public int id { get; set; }
        public int id_user { get; set; }
        public string kode { get; set; }
        public int id_vendor { get; set; }
        public string nama_produk { get; set; }
        public string no_tujuan { get; set; }
        public decimal harga_agen { get; set; }
        public string status_transaksi { get; set; }
        public string sn { get; set; }
        public int? id_pembeli { get; set; }
        public decimal? harga_jual { get; set; }
        public string tanggal_transaksi { get; set; }
        public string tanggal_konfirmasi { get; set; }
    }

    public class Payment
    {
        public string? merchantCode { get; set; }
        public string? reference { get; set; }
        public string? paymentUrl { get; set; }
        public string? qrString { get; set; }
        public string? amount { get; set; }
        public string? statusCode { get; set; }
        public string? statusMessage { get; set; }
    }

}
