using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Interface
{
    public interface IProdukService
    {
        Task SyncProdukAsync();
        Task<string> GetData();
        Task UpsertProdukAsync(DetailProduk produk);
        Task SyncProdukFromFileAsync(string fileName);
    }
}
