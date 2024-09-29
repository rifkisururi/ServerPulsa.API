namespace PedagangPulsa.API.Model
{
    public class OrderSumit
    {
        public int trx_id { get; set; }
        public int harga_jual { get; set; }
        public string nama_pembeli { get; set; }
        public string pin { get; set; }
    }
    public class OrderDrafModel
    {
        public int operator_Id { get; set; }
        public string kode { get; set; }
        public string no_tujuan { get; set; }
    }
}
