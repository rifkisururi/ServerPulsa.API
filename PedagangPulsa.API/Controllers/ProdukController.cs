using Microsoft.AspNetCore.Mvc;
using PedagangPulsa.API.Database;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdukController : ControllerBase
    {
        private readonly IProdukService _produkService;

        public ProdukController(IProdukService produkService)
        {
            _produkService = produkService;
        }

        // Endpoint untuk sinkronisasi data produk dari URL eksternal
        [HttpGet("sync")]
        public async Task<IActionResult> SyncProduk()
        {
            try
            {
                await _produkService.SyncProdukAsync();
                return Ok(new { Message = "Data produk berhasil disinkronisasi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Terjadi kesalahan", Error = ex.Message });
            }
        }

        // Endpoint untuk insert/update produk manual
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertProduk([FromBody] DetailProduk produk)
        {
            try
            {
                await _produkService.UpsertProdukAsync(produk);
                return Ok(new { Message = "Produk berhasil di-insert atau di-update" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Terjadi kesalahan", Error = ex.Message });
            }
        }

    }
}
