using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PedagangPulsa.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("ip")]
        public async Task<IActionResult> GetMyIp()
        {
            try
            {
                // Lakukan request ke API eksternal
                var response = await _httpClient.GetAsync("https://v4v6.ipv6-test.com/api/myip.php?json");

                // Cek apakah request sukses
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gagal mengambil data IP. StatusCode: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Terjadi kesalahan saat mengambil data IP.");
                }

                // Baca respons body
                var content = await response.Content.ReadAsStringAsync();

                // Kembalikan respons JSON secara langsung
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi pengecualian saat mengambil data IP.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Terjadi kesalahan server.");
            }
        }

        [HttpGet("cekProxy")]
        public async Task<IActionResult> CekProxy()
        {
            try
            {
                // Tentukan proxy, user, dan password
                var proxy = new WebProxy("http://proxy.rapidplex.com:3128")
                {
                    Credentials = new NetworkCredential("user", "HostingYaDomiNesia")
                };

                // Siapkan HttpClientHandler untuk menggunakan proxy
                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };

                using var client = new HttpClient(handler);

                // Lakukan request
                var response = await client.GetAsync("https://v4v6.ipv6-test.com/api/myip.php?json");
                if (!response.IsSuccessStatusCode)
                {
                    // Log error dan kembalikan status code dari response
                    _logger.LogError("Gagal memanggil API. StatusCode: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Gagal memanggil API.");
                }

                // Baca isi konten respons
                var content = await response.Content.ReadAsStringAsync();

                // Kembalikan konten JSON
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi pengecualian saat memanggil API via proxy.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Terjadi kesalahan server saat memanggil API via proxy.");
            }
        }
    }
}
