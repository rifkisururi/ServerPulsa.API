using System.ComponentModel.DataAnnotations;

namespace PedagangPulsa.API.Database.Entity
{
    public class DetailProduk
    {
        [Key]
        public int ProdukID { get; set; }
        public int KategoriID { get; set; }
        public int Harga { get; set; }
        public string Kode { get; set; }
        public string Nama { get; set; }
        public bool Status { get; set; }
    }

}
