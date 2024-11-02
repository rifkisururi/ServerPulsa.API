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
    public class HistoryController : Controller
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

        public HistoryController(IProdukService produkService, AppDbContext context, TokenService tokenService, ITransaksiService trxSvc, IDuitkuService duitkuService, ILogger<TransaksiController> logger, MqttService mqttService, IConfiguration configuration)
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

        [HttpGet("GetAllHistory")]
        public async Task<IActionResult> GetAllHistory(string supliyer)
        {
            string result = await _trxSvc.GetSaldo(supliyer);
            var jsonResponse = JsonSerializer.Deserialize<object>(result);
            return Ok(new
            {
                Status = "Success",
                Data = jsonResponse
            });
        }
    }
}
