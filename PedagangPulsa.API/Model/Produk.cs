namespace PedagangPulsa.API.Model
{
    public class KategoriProduk
    {
        public int KategoriID { get; set; }
        public string Provider { get; set; }
        public string Kategori { get; set; }
    }

    public class DetailProduk
    {
        public int ProdukID { get; set; }
        public int KategoriID { get; set; }
        public string Vendor { get; set; }
        public string Kode { get; set; }
        public string Nama { get; set; }
        public decimal Harga { get; set; }
        public int Status { get; set; }
        public KategoriProduk KategoriProduk { get; set; }
    }
}
