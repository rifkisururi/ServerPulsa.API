using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PedagangPulsa.API.Database;
using PedagangPulsa.API.Database.Entity;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;
using PedagangPulsa.API.Service;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace PedagangPulsa.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransaksiController : Controller
    {
        private readonly IProdukService _produkService;
        private readonly string _connectionString;
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly ITransaksiService _trxSvc;
        private readonly IDuitkuService _duitkuService; 
        private readonly ILogger<TransaksiController> _logger; 
        private readonly MqttService _mqttService; 
        private readonly IConfiguration _configuration;

        public TransaksiController(IProdukService produkService, AppDbContext context, TokenService tokenService, ITransaksiService trxSvc, IDuitkuService duitkuService, ILogger<TransaksiController> logger, MqttService mqttService, IConfiguration configuration)
        {
            _produkService = produkService;
            _connectionString = context.Database.GetDbConnection().ConnectionString; // Ambil connection string dari context
            _context = context;
            _tokenService = tokenService;
            _trxSvc = trxSvc;
            _duitkuService = duitkuService;
            _logger = logger; 
            _mqttService = mqttService;
            _configuration = configuration;

        }

        [HttpGet("saldo-supliyer/{supliyer}")]
        public async Task<IActionResult> saldoSupliyer(string supliyer)
        {
            string result = await _trxSvc.GetSaldo(supliyer);
            var jsonResponse = JsonSerializer.Deserialize<object>(result);
            return Ok(new
            {
                Status = "Success",
                Data = jsonResponse
            });
        }


        [HttpGet("operator-by-kategory/{kategory}")]
        public async Task<IActionResult> OperatorByKategory(string kategory)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                string query = @"
                    SELECT DISTINCT kategory_Id AS operator_Id, Provider, COUNT(1) as jml_active 
                    FROM produk
                    WHERE (Kategory = @Kategory OR (@Kategory = 'PULSA' AND Kategory = 'PULSA TRF') )  and status = TRUE 
                    group by Provider, Kategory
                    ";

                // Menggunakan Dapper untuk mengeksekusi query
                var produk = await connection.QueryAsync(query, new { Kategory = kategory });

                // Jika tidak ada produk yang ditemukan
                if (!produk.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = $"No products found for category: {kategory}",
                        Count = 0
                    });
                }

                // Return response JSON dengan format standar
                return Ok(new
                {
                    Status = "Success",
                    Message = $"Products found for category: {kategory}",
                    Count = produk.Count(),
                    Data = produk
                });
            }
        }

        [HttpGet("produk-by-operator/{operatorId}")]
        public async Task<IActionResult> ProdukByOperator(string operatorId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                string query = @"
                    select kategory_Id operator_Id, kode, nama_produk, deskripsi_produk, harga 
                    from produk 
                    where kategory_Id = @Operator and status = TRUE 
                    ";

                // Menggunakan Dapper untuk mengeksekusi query
                var produk = await connection.QueryAsync(query, new { Operator = operatorId });

                // Jika tidak ada produk yang ditemukan
                if (!produk.Any())
                {
                    return NotFound(new
                    {
                        Status = "Error",
                        Message = $"No products found for Operator: {operatorId}",
                        Count = 0
                    });
                }

                // Return response JSON dengan format standar
                return Ok(new
                {
                    Status = "Success",
                    Message = $"Products found for Operator: {operatorId}",
                    Count = produk.Count(),
                    Data = produk
                });
            }
        }

        [HttpPost("draf-order")]
        public async Task<IActionResult> draftOrder([FromBody] OrderDrafModel model)
        {
            var user = HttpContext.User;

            var dtProduk = await _context.Produk.SingleOrDefaultAsync(x => x.Kode == model.kode && x.kategory_Id == model.operator_Id);
            if (dtProduk == null)
            {
                return BadRequest(new
                {
                    Status = "Gagal",
                    Message = $"Terjadi kesalahan"
                });
            }
            else 
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    string query = @"
                    INSERT INTO Transaksi (IdUser, id_vendor, kode, nama_produk, harga_agen, status_transaksi, no_tujuan, TanggalTransaksi)
                    SELECT @userId, p.vendor, p.kode, p.nama_produk, p.harga, 'Draft', @tujuan, @datenow
                    FROM produk p 
                    WHERE p.kategory_Id = @Operator AND p.kode = @kode;

                    SELECT last_insert_rowid() AS Id;";

                    // Menggunakan service untuk validasi token
                    if (!_tokenService.ValidateTokenExpiration(user, out var userId, out var username, out var token, out var expirationClaim))
                    {
                        userId = "0";
                    }

                    // Execute the query and fetch the last inserted row ID
                    var lastInsertedId = await connection.QuerySingleAsync<int>(query, new
                    {
                        Operator = model.operator_Id,
                        kode = model.kode,
                        userId = userId,
                        tujuan = model.no_tujuan,
                        datenow = DateTime.Now,
                    });
                    // Return response JSON dengan format standar
                    return Ok(new
                    {
                        status = "Success",
                        trx_id = lastInsertedId
                    });
                }
            }
        }

        [HttpGet("get-draf-order/{transaksiId}")]
        public async Task<IActionResult> GetDraftOrder(string transaksiId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                string query = @"
                    SELECT t.kode, t.nama_produk, harga_agen, p.deskripsi_produk, no_tujuan , t.id
                    FROM Transaksi t
                    INNER JOIN produk p ON t.id_vendor = p.vendor AND t.kode = p.kode 
                    WHERE t.id = @trxId ;";

                // Menggunakan Dapper untuk mengeksekusi query
                var dt = await connection.QuerySingleAsync<Transaksi>(query, new { trxId = transaksiId });

                // Request Payment
                DuitkuPaymentRequest paymentRequest = new DuitkuPaymentRequest();
                paymentRequest.MerchantOrderId = transaksiId;
                paymentRequest.PaymentAmount = dt.harga_agen;
                paymentRequest.PhoneNumber = dt.no_tujuan;
                paymentRequest.ProductDetails = dt.nama_produk;
                
                var result = await _duitkuService.CreatePaymentRequestAsync(paymentRequest);
                // Return response JSON dengan format standar
                return Ok(new
                {
                    Status = "Success",
                    Message = $"Products found for Trx: {transaksiId}",
                    Data = dt,
                    Payment = result
                });
            }
        }


        [HttpGet("mqtt/{message}/{id}")]
        public async Task<IActionResult> mqtt(string message, string id)
        {
            string topic = "trxid"+id;
            await _mqttService.PublishMessageAsync(topic, message);

            return Ok(new { Status = "Message Published", Topic = topic, Message = message });
        }



        [HttpPost("callback")]
        public async Task<IActionResult> ReceiveCallback([FromForm] DuitkuCallbackRequest callbackRequest)
        {
            // Log the received callback
            _logger.LogInformation($"Received callback for MerchantOrderId: {callbackRequest.merchantOrderId}");

            // Calculate the expected signature for security validation
            var calculatedSignature = _duitkuService.generateSignatureCallback(callbackRequest.merchantCode, callbackRequest.merchantOrderId, callbackRequest.amount);

            // Validate the signature
            if (calculatedSignature != callbackRequest.signature)
            {
                _logger.LogWarning("Invalid signature detected!");
                return BadRequest(new { message = "Invalid signature" });
            }

            // Check the result code and handle the status
            if (callbackRequest.resultCode == "00")
            {
                // Success - Process the successful transaction
                _logger.LogInformation($"Transaction successful for MerchantOrderId: {callbackRequest.merchantOrderId}");
                OrderSumit model = new OrderSumit();
                string prefix = _configuration["Duitku:prefix"] + DateTime.Now.ToString("yyMM");
                model.trx_id = Convert.ToInt16(callbackRequest.merchantOrderId.Replace(prefix, ""));

                string topic = "trxid"+ model.trx_id;  // Ganti dengan topik yang sesuai
                string message = "paid";
                await _mqttService.PublishMessageAsync(topic, message);
                _trxSvc.SendTransaksi(model);
                return Ok(new { Status = "Message Published", Topic = topic, Message = message });
            }
            else
            {
                // Failed - Process the failed transaction
                _logger.LogError($"Transaction failed for MerchantOrderId: {callbackRequest.merchantOrderId}");
            }

            // Return a 200 OK status as the response
            return Ok(new { message = "Callback processed successfully" });
        }


        [HttpPost("submit-order")]
        public async Task<IActionResult> SubmitOrder([FromBody] OrderSumit model)
        {
            var a = await _trxSvc.SendTransaksi(model);
            return Ok(new
            {
                Status = "Success",
                Message = $"Products found for Trx: {model.trx_id}",
                Data = a
            });
        }

    }
}
