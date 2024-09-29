using PedagangPulsa.API.Model;

namespace PedagangPulsa.API.Interface
{
    public interface ITransaksiService
    {
        //Task SyncProdukAsync();
        //Task UpsertProdukAsync(DetailProduk produk);
        Task<string> GetSaldo(string supliyer);
        Task<object> SendTransaksi(OrderSumit dt);
    }
}
