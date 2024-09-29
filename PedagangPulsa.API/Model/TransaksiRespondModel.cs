namespace PedagangPulsa.API.Model
{
    public class TransaksiRespondDflash
    {
        public string refid { get; set; }         // Nomor referensi transaksi
        public bool Check { get; set; }           // Apakah transaksi telah dicek
        public bool Double { get; set; }          // Apakah transaksi merupakan duplikasi
        public DateTime TglEntri { get; set; }    // Tanggal transaksi masuk
        public DateTime TglStatus { get; set; }   // Tanggal status transaksi
        public string KodeProduk { get; set; }    // Kode produk yang ditransaksikan
        public string Tujuan { get; set; }        // Nomor tujuan
        public int status { get; set; }           // Status transaksi
        public string StatusText { get; set; }    // Deskripsi status transaksi
        public string message { get; set; }       // Pesan dari hasil transaksi
        public int Counter { get; set; }          // Counter untuk transaksi duplikat
        public string SN { get; set; }            // Serial number atau nomor transaksi
        public string Keterangan { get; set; }    // Keterangan tambahan (bisa kosong)
        public decimal Harga { get; set; }        // Harga transaksi
        public decimal Saldo { get; set; }        // Sisa saldo setelah transaksi
    }

    public class TransaksiRespondUser
    {
        public string trx_id { get; set; }         // Nomor referensi transaksi
        public string status { get; set; }           // Status transaksi
        
    }
}
