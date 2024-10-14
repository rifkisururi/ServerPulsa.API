using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PedagangPulsa.API.Database.Entity
{
    public class KategoriProduk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KategoriID { get; set; }
        public string Provider { get; set; }
        public string Kategori { get; set; }
    }

}
