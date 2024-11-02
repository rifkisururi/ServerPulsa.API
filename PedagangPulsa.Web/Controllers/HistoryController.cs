using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PedagangPulsa.Web.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly HttpClient _httpClient;
        private string _baseUrlApi = "https://localhost:7270/api/"; 
        public HistoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrlApi = configuration.GetSection("BaseUrlApi").Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
