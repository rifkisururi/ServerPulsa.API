namespace PedagangPulsa.API.Database.Entity
{
    public class Produk
    {
        public string Vendor { get; set; }           // Vendor yang menyediakan produk
        public string Kode { get; set; }             // Kode produk unik
        public int kategory_Id { get; set; }         // Kategori produk
        public string Kategory { get; set; }         // Kategori produk
        public string Provider { get; set; }         // Penyedia produk
        public string nama_produk { get; set; }       // Nama produk
        public string? deskripsi_produk { get; set; } // Deskripsi produk (opsional)
        public int Harga { get; set; }            // Harga produk
        public bool Status { get; set; }             // Status produk (aktif atau tidak)
    }

}
