using System.ComponentModel.DataAnnotations;

namespace PedagangPulsa.API.Database.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string NamaLengkap { get; set; }
        public string Alamat { get; set; }
        public string NoWA { get; set; }
        public string Email { get; set; }
        public string Telegram { get; set; }
        public string PinTransaksi { get; set; } = "000000";
        public string LastOTP { get; set; } = "000000";
        public int Saldo { get; set; } = 0;
        public int IsVerived { get; set; } = 0;
        public DateTime Register { get; set; }
        public DateTime LastTransaksi { get; set; } = DateTime.Now;
    }

}
