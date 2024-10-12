using System.ComponentModel.DataAnnotations;

namespace PedagangPulsa.API.Database.Entity
{
    public class Transaksi
    {
        [Key]
        public int id { get; set; }                   // ID transaksi
        public int IdUser { get; set; }               // ID user yang melakukan transaksi
        public string kode { get; set; }               // Kode produk
        public int id_vendor { get; set; }             // ID vendor penyedia produk
        public string nama_produk { get; set; }        // Nama produk
        public string no_tujuan { get; set; }          // Nomor tujuan transaksi
        public int harga_agen { get; set; }            // Harga yang dibayar oleh agen
        public string status_transaksi { get; set; }   // Status transaksi
        public string? sn { get; set; }   // Status transaksi
        public int? IdPembeli { get; set; }           // ID pembeli (opsional)
        public int? harga_jual { get; set; }           // Harga jual (opsional)
        public DateTime TanggalTransaksi { get; set; } = DateTime.Now;  // Tanggal transaksi
        public DateTime? TanggalKonfirmasi { get; set; }  // Tanggal konfirmasi transaksi (opsional)
    }

}
