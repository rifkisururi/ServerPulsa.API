using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using PedagangPulsa.API.Database.Entity;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PedagangPulsa.API.Service
{
    public class TransaksiService : ITransaksiService
    {
        private readonly string _connectionString;
        private static readonly HttpClient _httpClient = new HttpClient();
        private string query;

        private string _DflashId;
        private string _DflashPin;
        private string _DflashPwd;
        private string _DflashEndPont;


        public TransaksiService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            _DflashId = "DS018842";
            _DflashPin = "8435";
            _DflashPwd = "MajuJaya";
            _DflashEndPont = "http://api.dflash.co.id/";
        }

        public async Task<string> GetSaldo(string supliyer)
        {
            if (supliyer == "DFLASH")
            {
                string sign = CreateSign(string.Empty, string.Empty, string.Empty, _DflashId, _DflashPin, _DflashPwd);
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_DflashEndPont}balance?memberID={_DflashId}&sign={sign}");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return null;
        }
        public async Task<object> SendTransaksi(OrderSumit dt)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                query = @"  update Transaksi set status_transaksi = 'Request' where Id = @Id;
                            SELECT id_vendor, kode, nama_produk, no_tujuan FROM Transaksi WHERE Id = @Id";

                // Menggunakan Dapper untuk menjalankan query
                var result = await connection.QueryFirstOrDefaultAsync<Transaksi>(query, new { Id = dt.trx_id });
                string sign = CreateSign(result.kode, result.no_tujuan, result.id.ToString(), _DflashId, _DflashPin, _DflashPwd);
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_DflashEndPont}trx?memberID={_DflashId}&product={result.kode}&dest={result.no_tujuan}&refID={result.id.ToString()}&sign={sign}");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string resultHit = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<TransaksiRespondDflash>(resultHit);

                string statusTrx;
                TransaksiRespondUser respondUser = new TransaksiRespondUser();

                if (jsonResponse.message != null && jsonResponse.message.Contains("Your IP/Member ID/PIN/Password wrong!"))
                {
                    jsonResponse.message = "Hubungi Penyedia layanan";
                }
                if (jsonResponse.status == 0 || jsonResponse.status == 1 || jsonResponse.status == 2 || jsonResponse.status == 22)
                {
                    statusTrx = "Pending";
                }
                else if (jsonResponse.status == 20)
                {
                    statusTrx = "Sukses";
                }
                else
                {
                    statusTrx = "Gagal";
                }
                respondUser.status = " " + jsonResponse.message;
                respondUser.trx_id = dt.trx_id.ToString();

                query = @" update Transaksi set status_transaksi = @status, tanggal_konfirmasi = CURRENT_TIMESTAMP where Id = @Id;";
                await connection.ExecuteAsync(query, new { Id = dt.trx_id, status = statusTrx });

                return respondUser;
            }
        }

        private static string CreateSign(string product, string dest, string refID, string memberId, string pin, string password)
        {
            // Buat template string
            string template = $"OtomaX|{memberId}|{product}|{dest}|{refID}|{pin}|{password}";

            // Hash dengan SHA-1
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(template));

                // Encode ke Base64
                string base64 = Convert.ToBase64String(hash);

                // Ganti karakter sesuai dengan format yang diinginkan
                string encoded = base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');

                return encoded;
            }
        }
    }
}
