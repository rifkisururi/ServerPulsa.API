using System.ComponentModel.DataAnnotations;

namespace PedagangPulsa.API.Database.Entity
{
    public class KategoriProduk
    {
        [Key]
        public int KategoriID { get; set; }
        public string Provider { get; set; }
        public string Kategori { get; set; }
    }

}
