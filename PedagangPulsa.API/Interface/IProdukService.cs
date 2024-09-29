using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Interface
{
    public interface IProdukService
    {
        Task SyncProdukAsync();
        Task UpsertProdukAsync(DetailProduk produk);
    }
}
