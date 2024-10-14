using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedagangPulsa.API.Database.Entity
{
    public class DetailProduk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProdukID { get; set; }
        public int KategoriID { get; set; }
        public int Harga { get; set; }
        public string Kode { get; set; }
        public string Nama { get; set; }
        public bool Status { get; set; }
    }

}
